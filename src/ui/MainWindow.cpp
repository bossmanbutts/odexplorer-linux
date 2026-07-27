#include "MainWindow.hpp"
#include <QJsonDocument>
#include <QJsonObject>
#include <QJsonArray>
#include <QLocale>
#include <QMenuBar>
#include <QStatusBar>
#include <QVBoxLayout>
#include <QWidget>
#include <QTableWidget>
#include <QAbstractItemView>
#include <QHeaderView>
#include "../journal/JournalLocator.hpp"
#include "../journal/EventFormatter.hpp"

namespace {
const QString kJournalDirectory =
    "/mnt/twins/SteamLibrary/steamapps/compatdata/359320/pfx/drive_c/users/"
    "steamuser/Saved Games/Frontier Developments/Elite Dangerous/";
}

MainWindow::MainWindow(QWidget *parent) : QMainWindow(parent) {
  resize(1280, 720);
  setWindowTitle("OD Explorer Linux");

  auto *fileMenu = menuBar()->addMenu("&File");
  fileMenu->addAction("&Exit", this, &QWidget::close);

  auto *helpMenu = menuBar()->addMenu("&Help");
  helpMenu->addAction("&About");

  auto *central = new QWidget(this);
  auto *mainLayout = new QVBoxLayout(central);

  tabs_ = new QTabWidget;

  currentSystemTab_ = new QWidget;
  cartographyTab_ = new QWidget;
  exobiologyTab_ = new QWidget;

  auto *systemLayout = new QVBoxLayout(currentSystemTab_);
  auto *cartoLayout = new QVBoxLayout(cartographyTab_);
  auto *exoLayout = new QVBoxLayout(exobiologyTab_);

  tabs_->addTab(currentSystemTab_, "Current System");
  tabs_->addTab(cartographyTab_, "Cartography");
  tabs_->addTab(exobiologyTab_, "Exobiology");

  mainLayout->addWidget(tabs_);

  setCentralWidget(central);

  commanderLabel_ = new QLabel("Commander: Unknown");
  systemLabel_ = new QLabel("System: Unknown");
  shipLabel_ = new QLabel("Ship: Unknown");
  creditsLabel_ = new QLabel("Credits: 0 Cr");
  fuelLabel_ = new QLabel("Fuel: 0.0 / 0.0 t");
  explorationLabel_ = new QLabel("Exploration: 0 scanned, 0 mapped");

  logView_ = new QTextEdit;
  logView_->setReadOnly(true);

  systemLayout->addWidget(commanderLabel_);
  systemLayout->addWidget(systemLabel_);
  systemLayout->addWidget(shipLabel_);
  systemLayout->addWidget(creditsLabel_);
  systemLayout->addWidget(fuelLabel_);
  systemLayout->addWidget(explorationLabel_);
  systemLayout->addWidget(logView_);

  cartoTable_ = new QTableWidget(this);
  cartoTable_->setColumnCount(4);
  cartoTable_->setHorizontalHeaderLabels(
      {"Body", "Type", "Mapped", "Terraformable"});
  cartoTable_->setSortingEnabled(true);
  cartoTable_->setAlternatingRowColors(true);
  cartoTable_->setSelectionBehavior(QAbstractItemView::SelectRows);
  cartoTable_->setSelectionMode(QAbstractItemView::SingleSelection);
  cartoTable_->setEditTriggers(QAbstractItemView::NoEditTriggers);
  cartoTable_->horizontalHeader()->setStretchLastSection(true);
  cartoTable_->horizontalHeader()->setSectionResizeMode(
      QHeaderView::ResizeToContents);
  cartoDetailsLabel_ = new QLabel("Select a body");
  connect(cartoTable_, &QTableWidget::cellClicked, this,
          &MainWindow::onCartographicBodySelected);
  cartoLayout->addWidget(cartoTable_);
  cartoLayout->addWidget(cartoDetailsLabel_);

  exoLayout->addWidget(new QLabel("Exobiology database coming soon."));

  timer_ = new QTimer(this);

  connect(timer_, &QTimer::timeout, this, &MainWindow::updateJournal);

  timer_->start(1000);

  updateJournal();

  statusBar()->showMessage("Ready");
}

void MainWindow::updateJournal() {
  JournalLocator locator;

  QString journal = locator.latestJournal(kJournalDirectory);

  if (journal.isEmpty())
    return;

  if (!journalReader_ || journal != currentJournal_) {
    currentJournal_ = journal;

    journalReader_ = std::make_unique<JournalReader>(journal);

    if (!journalReader_->open())
      return;
  }

  QStringList lines = journalReader_->readNewLines();

  for (const QString &line : lines) {
    QJsonParseError error;

    QJsonDocument doc = QJsonDocument::fromJson(line.toUtf8(), &error);

    if (error.error != QJsonParseError::NoError)
      continue;

    if (!doc.isObject())
      continue;

    QJsonObject obj = doc.object();

    QString event = obj["event"].toString();
    
    if (event == "LoadGame") {
      gameState_.commander = obj["Commander"].toString();
      gameState_.ship = obj["Ship"].toString();
      gameState_.credits = obj["Credits"].toVariant().toLongLong();
    }

    else if (event == "Location") {
      gameState_.system = obj["StarSystem"].toString();
      gameState_.body = obj["Body"].toString();
      gameState_.station = obj["StationName"].toString();
      gameState_.docked = obj["Docked"].toBool();
      gameState_.inHyperspace = false;
      gameState_.resetExploration();
    }

    else if (event == "StartJump") {
      gameState_.destinationSystem = obj["StarSystem"].toString();

      gameState_.inHyperspace = true;
    }

    else if (event == "FSDJump") {
      logSystemSummary();
      gameState_.system = obj["StarSystem"].toString();

      if (obj.contains("Body"))
        gameState_.body = obj["Body"].toString();

      gameState_.station.clear();
      gameState_.docked = false;
      gameState_.inHyperspace = false;
      gameState_.inSupercruise = true;
      gameState_.resetExploration();
    }

    else if (event == "Docked") {
      gameState_.station = obj["StationName"].toString();
    }

    else if (event == "Undocked") {
      // nothing
    }

    else if (event == "SupercruiseEntry") {
      gameState_.inSupercruise = true;
    }

    else if (event == "SupercruiseExit") {
      gameState_.body = obj["Body"].toString();

      gameState_.inSupercruise = false;
    }

    else if (event == "ApproachBody") {
      gameState_.body = obj["Body"].toString();
    }

    else if (event == "Scan") {
      gameState_.scannedBodies++;

      CartographicBody body;
      body.name = obj["BodyName"].toString();

      if (obj.contains("PlanetClass")) {
        QString planet = obj["PlanetClass"].toString();
        bool terraformable =
            obj["TerraformState"].toString() == "Terraformable";

        body.type = planet;
        body.terraformable = terraformable;

        if (planet == "Earthlike body") {
          gameState_.earthLikes++;
          body.earthLike = true;
        }

        if (planet == "Water world") {
          gameState_.waterWorlds++;
          body.waterWorld = true;
        }

        if (planet == "Ammonia world") {
          gameState_.ammoniaWorlds++;
          body.ammoniaWorld = true;
        }

        if (terraformable)
          gameState_.terraformables++;
      } else if (obj.contains("StarType")) {
        body.type = obj["StarType"].toString();
      }

      bool found = false;

      for (auto &existing : gameState_.cartographicBodies) {
        if (existing.name == body.name) {
          existing = body;
          found = true;
          break;
        }
      }

      if (!found)
        bool found = false;

      for (auto &existing : gameState_.cartographicBodies) {
        if (existing.name == body.name) {
          existing = body;
          found = true;
          break;
        }
      }

      if (!found)
        gameState_.cartographicBodies.append(body);
    }

    else if (event == "SAAScanComplete") {
      gameState_.mappedBodies++;

      QString mappedBody = obj["BodyName"].toString();

      for (auto &body : gameState_.cartographicBodies) {
        if (body.name == mappedBody) {
          body.mapped = true;
          break;
        }
      }
    }

    else if (event == "FSSBodySignals") {
      QJsonArray signalArray = obj["Signals"].toArray();

      for (const QJsonValue &value : signalArray) {
        QJsonObject signal = value.toObject();

        if (signal["Type"].toString() == "$SAA_SignalType_Biological;") {
          gameState_.biologicalSignals += signal["Count"].toInt();
        }
      }
    }

    if (obj.contains("StarSystem"))
      gameState_.system = obj["StarSystem"].toString();

    if (obj.contains("BodyName"))
      gameState_.body = obj["BodyName"].toString();

    QString message = EventFormatter::format(obj, gameState_);

    if (!message.isEmpty()) 
    {
        logView_->append(message);
    }

    if (event == "Undocked")
        gameState_.station.clear();

    statusBar()->showMessage(
        QString("%1 | %2").arg(event).arg(gameState_.system));
  }

  statusReader_.read(gameState_);
  commanderLabel_->setText("Commander: " + gameState_.commander);
  systemLabel_->setText("System: " + gameState_.system);
  shipLabel_->setText("Ship: " + gameState_.ship);
  creditsLabel_->setText(
      QString("Credits: %1 Cr").arg(QLocale().toString(gameState_.credits)));
  fuelLabel_->setText(
      QString("Fuel: %1 / %2 t")
      .arg(gameState_.fuelMain, 0, 'f', 1)
      .arg(gameState_.fuelReservoir, 0, 'f', 1));
  explorationLabel_->setText(QString("Exploration\n"
                                     "Scanned: %1\n"
                                     "Mapped: %2\n"
                                     "Earth-like Worlds: %3\n"
                                     "Water Worlds: %4\n"
                                     "Ammonia Worlds: %5\n"
                                     "Terraformables: %6\n"
                                     "Biological Signals: %7")
                                 .arg(gameState_.scannedBodies)
                                 .arg(gameState_.mappedBodies)
                                 .arg(gameState_.earthLikes)
                                 .arg(gameState_.waterWorlds)
                                 .arg(gameState_.ammoniaWorlds)
                                 .arg(gameState_.terraformables)
                                 .arg(gameState_.biologicalSignals));
  refreshCartographyTable();
}

void MainWindow::logSystemSummary()
{
    if (gameState_.scannedBodies == 0 &&
        gameState_.mappedBodies == 0 &&
        gameState_.biologicalSignals == 0)
    {
        return;
    }

    logView_->append("");
    logView_->append(QString("=== %1 Summary ===").arg(gameState_.system));

    logView_->append(
        QString("%1 bodies scanned")
            .arg(gameState_.scannedBodies));

    logView_->append(
        QString("%1 bodies mapped")
            .arg(gameState_.mappedBodies));

    if (gameState_.earthLikes > 0)
        logView_->append(
            QString("%1 Earth-like World%2")
                .arg(gameState_.earthLikes)
                .arg(gameState_.earthLikes == 1 ? "" : "s"));

    if (gameState_.waterWorlds > 0)
        logView_->append(
            QString("%1 Water World%2")
                .arg(gameState_.waterWorlds)
                .arg(gameState_.waterWorlds == 1 ? "" : "s"));

    if (gameState_.ammoniaWorlds > 0)
        logView_->append(
            QString("%1 Ammonia World%2")
                .arg(gameState_.ammoniaWorlds)
                .arg(gameState_.ammoniaWorlds == 1 ? "" : "s"));

    if (gameState_.terraformables > 0)
        logView_->append(
            QString("%1 Terraformable%2")
                .arg(gameState_.terraformables)
                .arg(gameState_.terraformables == 1 ? "" : "s"));

    if (gameState_.biologicalSignals > 0)
        logView_->append(
            QString("%1 Biological Signal%2")
                .arg(gameState_.biologicalSignals)
                .arg(gameState_.biologicalSignals == 1 ? "" : "s"));

    logView_->append("");
}

void MainWindow::refreshCartographyTable() {
  cartoTable_->setSortingEnabled(false);

  cartoTable_->setRowCount(gameState_.cartographicBodies.size());

  for (int i = 0; i < gameState_.cartographicBodies.size(); ++i) {
    const auto &body = gameState_.cartographicBodies[i];

    auto *nameItem = new QTableWidgetItem(body.name);
    auto *typeItem = new QTableWidgetItem(body.type);
    auto *mappedItem = new QTableWidgetItem(body.mapped ? "Yes" : "");
    auto *terraformItem = new QTableWidgetItem(body.terraformable ? "Yes" : "");

    cartoTable_->setItem(i, 0, nameItem);
    cartoTable_->setItem(i, 1, typeItem);
    cartoTable_->setItem(i, 2, mappedItem);
    cartoTable_->setItem(i, 3, terraformItem);
  }

  cartoTable_->setSortingEnabled(true);
}

void MainWindow::onCartographicBodySelected(int row, int)
{
    if (row < 0 || row >= gameState_.cartographicBodies.size())
        return;

    const auto &body = gameState_.cartographicBodies[row];

    statusBar()->showMessage(
        QString("%1 (%2)")
            .arg(body.name)
            .arg(body.type));
}