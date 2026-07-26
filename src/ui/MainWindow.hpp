#pragma once

#include <QLabel>
#include <QMainWindow>
#include <QTextEdit>
#include <QTimer>
#include "../model/GameState.hpp"
#include <memory>
#include "../journal/JournalReader.hpp"
#include "../journal/StatusReader.hpp"
class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    explicit MainWindow(QWidget* parent = nullptr);

private:
    void updateJournal();

    QLabel* commanderLabel_;
    QLabel* systemLabel_;
    QLabel* shipLabel_;
    QLabel* creditsLabel_;

    QTextEdit* logView_;

    QTimer* timer_;

    std::unique_ptr<JournalReader> journalReader_;
    StatusReader statusReader_;
    QString currentJournal_;
    GameState gameState_;
};