#include "MainWindow.hpp"

#include <QMenuBar>
#include <QStatusBar>

MainWindow::MainWindow(QWidget* parent)
    : QMainWindow(parent)
{
    resize(1280, 720);

    setWindowTitle("OD Explorer Linux");

    auto* fileMenu = menuBar()->addMenu("&File");
    fileMenu->addAction("&Exit", this, &QWidget::close);

    auto* helpMenu = menuBar()->addMenu("&Help");
    helpMenu->addAction("&About");

    statusBar()->showMessage("Ready");
}