#include "MainWindow.hpp"
#include "../journal/EventFormatter.hpp"
#include "../journal/JournalLocator.hpp"
#include "../exploration/ExplorationValue.hpp"
#include <QAbstractItemView>
#include <QHeaderView>
#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>
#include <QLocale>
#include <QMenuBar>
#include <QStatusBar>
#include <QTableWidget>
#include <QVBoxLayout>
#include <QWidget>

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

  auto *cartoSplit = new QHBoxLayout;

  cartoTable_ = new QTableWidget(this);
  cartoTable_->setColumnCount(6);
  cartoTable_->setHorizontalHeaderLabels(
      {"Body", "Type", "Mapped", "Bio", "Scan Value", "Exobio Value"});
  cartoTable_->horizontalHeader()->setStretchLastSection(true);
  cartoTable_->setSelectionBehavior(QAbstractItemView::SelectRows);
  cartoTable_->setSelectionMode(QAbstractItemView::SingleSelection);
  cartoTable_->setEditTriggers(QAbstractItemView::NoEditTriggers);
  cartoTable_->setSortingEnabled(true);

  connect(cartoTable_, &QTableWidget::cellClicked, this,
          &MainWindow::onCartographicBodySelected);

  auto *detailsWidget = new QWidget;
  auto *detailsLayout = new QVBoxLayout(detailsWidget);

  bodyNameLabel_ = new QLabel("Name:");
  bodyTypeLabel_ = new QLabel("Type:");
  bodyMappedLabel_ = new QLabel("Mapped:");
  bodyTerraformableLabel_ = new QLabel("Terraformable:");
  bodyValueLabel_ = new QLabel("Estimated Value:");
  cartoDetailsLabel_ = new QLabel("Exploration: 0 Cr\nExobiology: 0 Cr");
  bodyLandableLabel_ = new QLabel("Landable:");
  bodyGravityLabel_ = new QLabel("Gravity:");
  bodyTemperatureLabel_ = new QLabel("Temperature:");
  bodyAtmosphereLabel_ = new QLabel("Atmosphere:");
  bodyVolcanismLabel_ = new QLabel("Volcanism:");
  bodyBioSignalsLabel_ = new QLabel("Biological Signals:");

  detailsLayout->addWidget(bodyNameLabel_);
  detailsLayout->addWidget(bodyTypeLabel_);
  detailsLayout->addWidget(bodyMappedLabel_);
  detailsLayout->addWidget(bodyTerraformableLabel_);
  detailsLayout->addWidget(bodyValueLabel_);
  detailsLayout->addWidget(cartoDetailsLabel_);
  detailsLayout->addWidget(bodyLandableLabel_);
  detailsLayout->addWidget(bodyGravityLabel_);
  detailsLayout->addWidget(bodyTemperatureLabel_);
  detailsLayout->addWidget(bodyAtmosphereLabel_);
  detailsLayout->addWidget(bodyVolcanismLabel_);
  detailsLayout->addWidget(bodyBioSignalsLabel_);
  detailsLayout->addStretch();
  cartoSplit->addWidget(cartoTable_, 3);
  cartoSplit->addWidget(detailsWidget, 1);

  cartoLayout->addLayout(cartoSplit);

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
        body.landable = obj["Landable"].toBool();
        body.radius = obj["Radius"].toDouble();
        body.gravity = obj["SurfaceGravity"].toDouble();
        body.temperature = obj["SurfaceTemperature"].toDouble();
        body.atmosphere = obj["AtmosphereType"].toString();
        body.volcanism = obj["Volcanism"].toString();
        body.massEM = obj["MassEM"].toDouble();
        body.distanceFromArrivalLS = obj["DistanceFromArrivalLS"].toDouble();
        body.firstDiscovery = !obj["WasDiscovered"].toBool();
        body.firstMapped = !obj["WasMapped"].toBool();

        body.estimatedValue = ExplorationValue::calculateBodyValue(body);

        if (planet == "Earthlike body")
          gameState_.earthLikes++;

        if (planet == "Water world")
          gameState_.waterWorlds++;

        if (planet == "Ammonia world")
          gameState_.ammoniaWorlds++;

        if (terraformable)
          gameState_.terraformables++;
      } else if (obj.contains("StarType")) {
        body.type = obj["StarType"].toString();

        body.firstDiscovery = !obj["WasDiscovered"].toBool();
        body.firstMapped = !obj["WasMapped"].toBool();

        body.estimatedValue = ExplorationValue::calculateBodyValue(body);
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
        gameState_.cartographicBodies.append(body);
    }

    else if (event == "SAAScanComplete") {
      gameState_.mappedBodies++;

      const QString bodyName = obj["BodyName"].toString();

      for (auto &body : gameState_.cartographicBodies) {
        if (body.name != bodyName)
          continue;

        body.mapped = true;

        if (obj.contains("EfficiencyTarget"))
          body.efficiencyTarget = obj["EfficiencyTarget"].toInt();

        if (obj.contains("ProbesUsed"))
          body.probesUsed = obj["ProbesUsed"].toInt();

        body.efficiencyBonus = obj["EfficiencyTargetAchieved"].toBool();

        body.estimatedValue = ExplorationValue::calculateBodyValue(body);

        break;
      }
    }

    else if (event == "FSSBodySignals") {
      const QString bodyName = obj["BodyName"].toString();

      for (auto &body : gameState_.cartographicBodies) {
        if (body.name != bodyName)
          continue;

        body.biologicalSignals = 0;
        body.genera.clear();

        if (obj.contains("Signals")) {
          for (const auto &value : obj["Signals"].toArray()) {
            const auto signal = value.toObject();

            const QString type = signal["Type"].toString();
            const int count = signal["Count"].toInt();

            if (!type.contains("Biological"))
              continue;

            body.biologicalSignals += count;

            QString genus = type.section('_', -1);

            if (!body.genera.contains(genus))
              body.genera.append(genus);
          }
        }

        body.genusCount = body.genera.size();

        body.estimatedExobiologyValue =
            ExplorationValue::calculateExobiologyValue(body);

        gameState_.biologicalSignals = 0;

        for (const auto &b : gameState_.cartographicBodies)
          gameState_.biologicalSignals += b.biologicalSignals;

        break;
      }
    }

    if (obj.contains("StarSystem"))
      gameState_.system = obj["StarSystem"].toString();

    if (obj.contains("BodyName"))
      gameState_.body = obj["BodyName"].toString();

    QString message = EventFormatter::format(obj, gameState_);

    if (!message.isEmpty()) {
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
  fuelLabel_->setText(QString("Fuel: %1 / %2 t")
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

void MainWindow::logSystemSummary() {
  if (gameState_.scannedBodies == 0 && gameState_.mappedBodies == 0 &&
      gameState_.biologicalSignals == 0) {
    return;
  }

  logView_->append("");
  logView_->append(QString("=== %1 Summary ===").arg(gameState_.system));

  logView_->append(QString("%1 bodies scanned").arg(gameState_.scannedBodies));

  logView_->append(QString("%1 bodies mapped").arg(gameState_.mappedBodies));

  if (gameState_.earthLikes > 0)
    logView_->append(QString("%1 Earth-like World%2")
                         .arg(gameState_.earthLikes)
                         .arg(gameState_.earthLikes == 1 ? "" : "s"));

  if (gameState_.waterWorlds > 0)
    logView_->append(QString("%1 Water World%2")
                         .arg(gameState_.waterWorlds)
                         .arg(gameState_.waterWorlds == 1 ? "" : "s"));

  if (gameState_.ammoniaWorlds > 0)
    logView_->append(QString("%1 Ammonia World%2")
                         .arg(gameState_.ammoniaWorlds)
                         .arg(gameState_.ammoniaWorlds == 1 ? "" : "s"));

  if (gameState_.terraformables > 0)
    logView_->append(QString("%1 Terraformable%2")
                         .arg(gameState_.terraformables)
                         .arg(gameState_.terraformables == 1 ? "" : "s"));

  if (gameState_.biologicalSignals > 0)
    logView_->append(QString("%1 Biological Signal%2")
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

    cartoTable_->setItem(i, 0, nameItem);
    cartoTable_->setItem(i, 1, typeItem);
    cartoTable_->setItem(i, 2, mappedItem);
  }

  cartoTable_->setSortingEnabled(true);
}

void MainWindow::onCartographicBodySelected(int row, int) {
  if (row < 0 || row >= gameState_.cartographicBodies.size())
    return;

  const auto &body = gameState_.cartographicBodies[row];

  bodyNameLabel_->setText(body.name);

  bodyTypeLabel_->setText(QString("Type: %1").arg(body.type));

  bodyMappedLabel_->setText(
      QString("Mapped: %1").arg(body.mapped ? "Yes" : "No"));

  bodyTerraformableLabel_->setText(
      QString("Terraformable: %1").arg(body.terraformable ? "Yes" : "No"));

  bodyValueLabel_->setText(QString("Estimated Value: %1 Cr")
                               .arg(QLocale().toString(body.estimatedValue)));

  cartoDetailsLabel_->setText(
      QString("Exploration: %1 Cr\nExobiology: %2 Cr")
          .arg(QLocale().toString(body.estimatedValue))
          .arg(QLocale().toString(body.estimatedExobiologyValue)));

  bodyLandableLabel_->setText(
      QString("Landable: %1").arg(body.landable ? "Yes" : "No"));

  bodyGravityLabel_->setText(
      QString("Gravity: %1 G").arg(body.gravity / 9.80665, 0, 'f', 2));

  bodyTemperatureLabel_->setText(
      QString("Temperature: %1 K").arg(body.temperature, 0, 'f', 0));

  bodyAtmosphereLabel_->setText(
      QString("Atmosphere: %1")
          .arg(body.atmosphere.isEmpty() ? "None" : body.atmosphere));

  bodyVolcanismLabel_->setText(
      QString("Volcanism: %1")
          .arg(body.volcanism.isEmpty() ? "None" : body.volcanism));

  bodyBioSignalsLabel_->setText(
      QString("Biological Signals: %1").arg(body.biologicalSignals));
}