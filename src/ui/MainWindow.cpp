#include "MainWindow.hpp"

#include <QMenuBar>
#include <QStatusBar>
#include <QVBoxLayout>
#include <QWidget>

#include "../journal/JournalLocator.hpp"

#include <QFile>
#include <QJsonDocument>
#include <QJsonObject>
#include <QTextStream>
#include <QTimer>

static QString readLastLine(const QString& filePath)
{
    QFile file(filePath);

    if (!file.open(QIODevice::ReadOnly | QIODevice::Text))
        return {};

    QTextStream stream(&file);

    QString lastLine;

    while (!stream.atEnd())
    {
        QString line = stream.readLine();

        if (!line.trimmed().isEmpty())
            lastLine = line;
    }

    return lastLine;
}

MainWindow::MainWindow(QWidget* parent)
    : QMainWindow(parent)
{
    resize(1280, 720);

    setWindowTitle("OD Explorer Linux");

    auto* fileMenu = menuBar()->addMenu("&File");
    fileMenu->addAction("&Exit", this, &QWidget::close);

    auto* helpMenu = menuBar()->addMenu("&Help");
    helpMenu->addAction("&About");

    auto* central = new QWidget(this);
    auto* layout = new QVBoxLayout(central);

    journalLabel_ = new QLabel("Current Journal");
    logView_ = new QTextEdit();

    logView_->setReadOnly(true);

    layout->addWidget(journalLabel_);
    layout->addWidget(logView_);

    setCentralWidget(central);

    timer_ = new QTimer(this);

    connect(timer_,
            &QTimer::timeout,
            this,
            &MainWindow::updateJournal);

    timer_->start(1000);

    updateJournal();

    statusBar()->showMessage("Ready");
}

void MainWindow::updateJournal()
{
    JournalLocator locator;

    QString journal = locator.latestJournal(
        "/mnt/twins/SteamLibrary/steamapps/compatdata/359320/pfx/drive_c/users/steamuser/Saved Games/Frontier Developments/Elite Dangerous/");

    if (journal.isEmpty())
        return;

    QString lastLine = readLastLine(journal);

    if (lastLine == lastJournalLine_)
        return;

    lastJournalLine_ = lastLine;

    QJsonParseError error;
    QJsonDocument doc =
        QJsonDocument::fromJson(lastLine.toUtf8(), &error);

    if (error.error != QJsonParseError::NoError || !doc.isObject())
        return;

    QJsonObject obj = doc.object();

    QString eventName = obj["event"].toString();

    journalLabel_->setText(
        "Journal:\n" + journal +
        "\n\nLast Event:\n" + eventName);

    logView_->setPlainText(lastLine);

    statusBar()->showMessage(eventName);
}