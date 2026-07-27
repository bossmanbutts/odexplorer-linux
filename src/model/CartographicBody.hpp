#pragma once

#include <QString>

struct CartographicBody
{
    QString name;
    QString type;

    bool mapped = false;
    bool terraformable = false;

    bool earthLike = false;
    bool waterWorld = false;
    bool ammoniaWorld = false;

    bool landable = false;

    QString atmosphere;
    QString volcanism;

    double gravity = 0.0;
    double surfaceTemperature = 0.0;
    double temperature = 0.0;

    int biologicalSignals = 0;

    qint64 value = 0;
};