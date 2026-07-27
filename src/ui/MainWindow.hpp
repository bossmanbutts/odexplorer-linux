#pragma once

#include <QLabel>
#include <QMainWindow>
#include <QTextEdit>
#include <QTimer>
#include <QTabWidget>
#include <QTableWidget>
#include <QHeaderView>
#include <memory>
#include "../model/GameState.hpp"
#include "../journal/JournalReader.hpp"
#include "../journal/StatusReader.hpp"
class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    explicit MainWindow(QWidget* parent = nullptr);

private:
    void updateJournal();
    void logSystemSummary();
    void refreshCartographyTable();
    void onCartographicBodySelected(int row, int column);

    QLabel *commanderLabel_;
    QLabel *systemLabel_;
    QLabel *shipLabel_;
    QLabel *creditsLabel_;
    QLabel *fuelLabel_;
    QLabel *explorationLabel_;
    QLabel *cartoDetailsLabel_;
    QLabel *bodyNameLabel_;
    QLabel *bodyTypeLabel_;
    QLabel *bodyMappedLabel_;
    QLabel *bodyTerraformableLabel_;
    QLabel *bodyLandableLabel_;
    QLabel *bodyGravityLabel_;
    QLabel *bodyTemperatureLabel_;
    QLabel *bodyAtmosphereLabel_;
    QLabel *bodyVolcanismLabel_;
    QLabel *bodyBioSignalsLabel_;
    QLabel *bodyValueLabel_;

    QTextEdit* logView_;

    QTimer* timer_;

    QTabWidget* tabs_;
    QTableWidget *cartoTable_;
    QWidget *currentSystemTab_;
    QWidget *cartographyTab_;
    QWidget *exobiologyTab_;

    std::unique_ptr<JournalReader> journalReader_;
    StatusReader statusReader_;
    QString currentJournal_;
    GameState gameState_;
};