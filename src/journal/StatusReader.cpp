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

    return true;
}