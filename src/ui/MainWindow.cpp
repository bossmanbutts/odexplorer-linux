#include "MainWindow.hpp"

#include <QJsonDocument>
#include <QJsonObject>
#include <QLocale>
#include <QMenuBar>
#include <QStatusBar>
#include <QVBoxLayout>
#include <QWidget>

#include "../journal/JournalLocator.hpp"

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
  auto *layout = new QVBoxLayout(central);

  commanderLabel_ = new QLabel("Commander: Unknown");
  systemLabel_ = new QLabel("System: Unknown");
  shipLabel_ = new QLabel("Ship: Unknown");
  creditsLabel_ = new QLabel("Credits: 0 Cr");

  logView_ = new QTextEdit;
  logView_->setReadOnly(true);

  layout->addWidget(commanderLabel_);
  layout->addWidget(systemLabel_);
  layout->addWidget(shipLabel_);
  layout->addWidget(creditsLabel_);
  layout->addWidget(logView_);

  setCentralWidget(central);

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
    }

    else if (event == "StartJump") {
      gameState_.destinationSystem = obj["StarSystem"].toString();

      gameState_.inHyperspace = true;
    }

    else if (event == "FSDJump") {
      gameState_.system = obj["StarSystem"].toString();

      if (obj.contains("Body"))
        gameState_.body = obj["Body"].toString();

      gameState_.station.clear();

      gameState_.docked = false;
      gameState_.inHyperspace = false;
      gameState_.inSupercruise = true;
    }

    else if (event == "Docked") {
      gameState_.station = obj["StationName"].toString();

      gameState_.docked = true;
    }

    else if (event == "Undocked") {
      gameState_.station.clear();

      gameState_.docked = false;
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
    if (obj.contains("StarSystem"))
      gameState_.system = obj["StarSystem"].toString();

    if (obj.contains("BodyName"))
      gameState_.body = obj["BodyName"].toString();

    commanderLabel_->setText("Commander: " + gameState_.commander);

    systemLabel_->setText("System: " + gameState_.system);

    shipLabel_->setText("Ship: " + gameState_.ship);

    creditsLabel_->setText(
        QString("Credits: %1 Cr").arg(QLocale().toString(gameState_.credits)));

    logView_->setPlainText(line);

    statusBar()->showMessage(
        QString("%1 | %2").arg(event).arg(gameState_.system));
  }

  statusReader_.read(gameState_);
  qDebug() << gameState_.fuelMain
         << gameState_.fuelReservoir;

  commanderLabel_->setText("Commander: " + gameState_.commander);
  systemLabel_->setText("System: " + gameState_.system);
  shipLabel_->setText("Ship: " + gameState_.ship);

  creditsLabel_->setText(
      QString("Fuel: %1 / %2 t")
      .arg(gameState_.fuelMain, 0, 'f', 1)
      .arg(gameState_.fuelReservoir, 0, 'f', 1));
}