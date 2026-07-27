#pragma once

#include <QString>
#include "CartographicBody.hpp"
#include <QVector>

struct GameState
{
    QString commander;
    QString system;
    QString body;
    QString station;
    QString ship;
    QString destinationSystem;
    QVector<CartographicBody> cartographicBodies;
    qint64 credits = 0;
    
    double fuelMain = 0;
    double fuelReservoir = 0;

    bool docked = false;
    bool inSupercruise = false;
    bool inHyperspace = false;
    bool landingGear = false;
    bool lights = false;
    bool cargoScoop = false;
    bool silentRunning = false;
    bool flightAssistOff = false;
    bool hardpointsDeployed = false;
    bool inWing = false;
    bool shieldsUp = true;
    bool supercruiseAssist = false;
    bool dockingComputer = false;
    bool massLocked = false;
    bool fsdCharging = false;
    bool fsdCooldown = false;
    bool lowFuel = false;
    bool overheating = false;
    bool nightVision = false;
    bool analysisMode = false;
    bool srv = false;
    bool fighter = false;
    bool onFoot = false;

    int cargo = 0;
    int scannedBodies = 0;
    int mappedBodies = 0;
    int earthLikes = 0;
    int waterWorlds = 0;
    int ammoniaWorlds = 0;
    int terraformables = 0;
    int biologicalSignals = 0;

    void resetExploration();
    
};