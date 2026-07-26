#pragma once

#include <QString>

struct GameState
{
    QString commander;

    QString system;
    QString body;
    QString station;

    QString ship;

    QString destinationSystem;

    qint64 credits = 0;

    bool docked = false;
    bool inSupercruise = false;
    bool inHyperspace = false;

    double fuelMain = 0;
    double fuelReservoir = 0;

    int cargo = 0;
};