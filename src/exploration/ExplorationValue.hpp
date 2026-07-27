#pragma once

#include "../model/CartographicBody.hpp"

class ExplorationValue {
public:
  static int calculateBodyValue(const CartographicBody &body);
  static int calculateExobiologyValue(const CartographicBody &body);

private:
  static double basePlanetValue(double k, double mass);
};