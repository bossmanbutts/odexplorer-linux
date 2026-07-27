#include "StatusReader.hpp"

#include <QFile>
#include <QJsonDocument>
#include <QJsonObject>

namespace
{
const QString kStatusFile =
    "/mnt/twins/SteamLibrary/steamapps/compatdata/359320/pfx/drive_c/users/"
    "steamuser/Saved Games/Frontier Developments/Elite Dangerous/Status.json";
}

bool StatusReader::read(GameState& state)
{
    QFile file(kStatusFile);

    if (!file.open(QIODevice::ReadOnly))
        return false;

    QJsonParseError error;

    QJsonDocument doc =
        QJsonDocument::fromJson(file.readAll(), &error);

    if (error.error != QJsonParseError::NoError)
        return false;

    if (!doc.isObject())
        return false;

    QJsonObject obj = doc.object();

    if (obj.contains("Fuel"))
    {
        QJsonObject fuel = obj["Fuel"].toObject();

        state.fuelMain =
            fuel["FuelMain"].toDouble();

        state.fuelReservoir =
            fuel["FuelReservoir"].toDouble();
    }

    if (obj.contains("Cargo"))
        state.cargo = obj["Cargo"].toInt();

    if (obj.contains("Flags")) 
    {
        int flags = obj["Flags"].toInt();

        state.landingGear = flags & (1 << 0);
        state.shieldsUp = !(flags & (1 << 1));
        state.supercruiseAssist = flags & (1 << 4);
        state.flightAssistOff = flags & (1 << 5);
        state.hardpointsDeployed = flags & (1 << 6);
        state.inWing = flags & (1 << 7);
        state.lights = flags & (1 << 8);
        state.cargoScoop = flags & (1 << 9);
        state.silentRunning = flags & (1 << 10);
        state.srv = flags & (1 << 26);
        state.fighter = flags & (1 << 27);
        state.massLocked = flags & (1 << 30);
        state.fsdCharging = flags & (1 << 31);
    }

    if (obj.contains("Flags2"))
    {
        int flags2 = obj["Flags2"].toInt();

        state.onFoot = flags2 & (1 << 0);
        state.analysisMode = flags2 & (1 << 1);
        state.nightVision = flags2 & (1 << 4);
        state.lowFuel = flags2 & (1 << 10);
        state.overheating = flags2 & (1 << 11);
        state.fsdCooldown = flags2 & (1 << 21);
    }

    return true;
}