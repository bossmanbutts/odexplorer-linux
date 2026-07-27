#include "ExplorationValue.hpp"

#include <algorithm>
#include <cmath>

double ExplorationValue::basePlanetValue(double k, double mass) {
  constexpr double q = 0.56591828;

  return std::max(k + (k * std::pow(mass, 0.2) * q), 500.0);
}

int ExplorationValue::calculateBodyValue(const CartographicBody &body) {
  double k;

  if (body.type.contains("Star", Qt::CaseInsensitive)) {
    double starValue = basePlanetValue(1200.0, std::max(body.massEM, 1.0));

    if (body.firstDiscovery)
      starValue *= 2.6;

    if (body.mapped) {
      starValue *= 3.699622554;

      if (body.efficiencyBonus)
        starValue *= 1.25;
    }

    return static_cast<int>(std::round(starValue));
  }

  if (body.type == "Earthlike body") {
    k = 64831 + 116295;
  } else if (body.type == "Water world") {
    k = 64831;

    if (body.terraformable)
      k += 116295;
  } else if (body.type == "Ammonia world") {
    k = 96932;
  } else if (body.type == "Metal-rich body") {
    k = 21790;

    if (body.terraformable)
      k += 65631;
  } else if (body.type == "High metal content body") {
    k = 9654;

    if (body.terraformable)
      k += 100677;
  } else {
    k = 500;

    if (body.terraformable)
      k += 93328;
  }

  double value = basePlanetValue(k, body.massEM);
  constexpr double kFirstDiscoveryBonus = 2.6;
  constexpr double kMappedMultiplier = 3.699622554;
  constexpr double kEfficiencyBonus = 1.25;

  if (body.firstDiscovery)
    value *= kFirstDiscoveryBonus;

  if (body.mapped) {
    value *= kMappedMultiplier;

    if (body.efficiencyBonus)
      value *= kEfficiencyBonus;
  }

  return static_cast<int>(value);
}

int ExplorationValue::calculateExobiologyValue(const CartographicBody &body) {
  return body.biologicalSignals * 500000;
}