#pragma once

#include <QJsonObject>
#include <QString>
#include "../model/GameState.hpp"

class EventFormatter
{
public:
  static QString format(const QJsonObject &obj, const GameState &state);
};