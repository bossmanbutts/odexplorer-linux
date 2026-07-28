#include "ExplorationValue.hpp"

#include <algorithm>
#include <cmath>
#include <QDebug>

double ExplorationValue::odysseyValue(double value) {
  return value + std::max(value * 0.3, 555.0);
}

double ExplorationValue::starValue33(double k, double mass) {
  return k + (mass * k / 66.25);
}

double ExplorationValue::planetValue33(double k, double mass) {
  constexpr double q = 0.56591828;
  return std::max(k + (k * std::pow(mass, 0.2) * q), 500.0);
}

double ExplorationValue::planetValue32(double k, double mass) {
  return k + (3.0 * k * std::pow(mass, 0.199977) / 5.3);
}

ExplorationValue::EstimatedValues
ExplorationValue::calculateEstimatedValues(const CartographicBody &body) {
  EstimatedValues out;

  //
  // 3.3+ STAR VALUES
  //
  if (body.isStar) {
    double k;

    const QString t = body.type.toUpper();

    // White dwarfs
    if (t == "D" || t.startsWith("DA") || t.startsWith("DB") ||
        t.startsWith("DC") || t.startsWith("DO") || t.startsWith("DQ") ||
        t.startsWith("DX")) {
      k = 33737.0;
    }
    // Neutron stars / Black holes
    else if (t == "N" || t == "H") {
      k = 54309.0;
    }
    // Main sequence
    else if (t == "O") {
      k = 4170.0;
    } else if (t == "B") {
      k = 3098.0;
    } else if (t == "A") {
      k = 2950.0;
    } else if (t == "F") {
      k = 2932.0;
    } else if (t == "G") {
      k = 2923.0;
    } else if (t == "K") {
      k = 2911.0;
    } else if (t == "M") {
      k = 2911.0;
    }
    // Brown dwarfs
    else if (t == "L") {
      k = 2887.0;
    } else if (t == "T") {
      k = 2883.0;
    } else if (t == "Y") {
      k = 2881.0;
    }
    // Wolf-Rayet
    else if (t.startsWith("W")) {
      k = 7794.0;
    }
    // Carbon stars
    else if (t.startsWith("C")) {
      k = 2920.0;
    } else {
      k = 2000.0;
    }

    const double base = starValue33(k, std::max(body.stellarMass, 1.0));

    out.base = (int)base;
    out.firstDiscovery = (int)(base * 2.6);

    return out;
  }

  //
  // Asteroid belts
  //
  if (body.type.isEmpty())
    return out;

  //
  // 3.3+ PLANET VALUES
  //
  double k = 300.0;

  if (body.type == "Metal rich body") {
    k = 21790;
    if (body.terraformable)
      k += 65631;
  } else if (body.type == "High metal content body") {
    k = 9654;
    if (body.terraformable)
      k += 100677;
  } else if (body.type == "Sudarsky class II gas giant") {
    k = 9654;
  } else if (body.type == "Earthlike body") {
    k = 64831 + 116295;
  } else if (body.type == "Water world") {
    k = 64831;
    if (body.terraformable)
      k += 116295;
  } else if (body.type == "Ammonia world") {
    k = 96932;
  } else if (body.type == "Sudarsky class I gas giant") {
    k = 1656;
  } else {
    k = 300;
    if (body.terraformable)
      k += 93328;
  }

  const double mass = std::max(body.massEM, 1.0);
  const double base = planetValue33(k, mass);

  constexpr double eff = 1.25;
  constexpr double first = 2.6;
  constexpr double mapFD = 3.699622554;
  constexpr double mapFM = 8.0956;
  constexpr double mapMapped = 3.3333333;

  out.base = (int)base;
  out.firstDiscovery = (int)(base * first);

  out.firstDiscoveryFirstMapped = (int)(odysseyValue(base * mapFD) * first);

  out.firstDiscoveryFirstMappedEfficiently =
      (int)(odysseyValue(base * mapFD) * first * eff);

  out.firstMapped = (int)(odysseyValue(base * mapFM));

  out.firstMappedEfficiently = (int)(odysseyValue(base * mapFM) * eff);

  out.mapped = (int)(odysseyValue(base * mapMapped));

  out.mappedEfficiently = (int)(odysseyValue(base * mapMapped) * eff);

  return out;
}

int ExplorationValue::calculateBodyValue(const CartographicBody &body)
{
    const EstimatedValues values = calculateEstimatedValues(body);

    // Stars
    if (body.isStar)
    {
        return body.firstDiscovery
            ? values.firstDiscovery
            : values.base;
    }

    const bool wasNotPreviouslyDiscovered = body.firstDiscovery;
    const bool wasNotPreviouslyMapped = body.firstMapped;

    if (wasNotPreviouslyDiscovered &&
        !body.mapped &&
        !body.firstMapped)
    {
        return values.base;
    }

    if (wasNotPreviouslyDiscovered &&
        body.mapped &&
        !body.firstMapped)
    {
        return body.efficiencyBonus
            ? values.mappedEfficiently
            : values.mapped;
    }

    if (wasNotPreviouslyDiscovered &&
        wasNotPreviouslyMapped &&
        body.mapped)
    {
        return body.efficiencyBonus
            ? values.firstDiscoveryFirstMappedEfficiently
            : values.firstDiscoveryFirstMapped;
    }

    if (wasNotPreviouslyMapped &&
        body.mapped)
    {
        return body.efficiencyBonus
            ? values.firstMappedEfficiently
            : values.firstMapped;
    }

    if (wasNotPreviouslyDiscovered)
    {
        return values.firstDiscovery;
    }

    if (body.mapped)
    {
      return body.efficiencyBonus ? values.mappedEfficiently : values.mapped;
    }

    qDebug() << body.name << "mapped=" << body.mapped
             << "firstMapped=" << body.firstMapped
             << "firstDiscovery=" << body.firstDiscovery
             << "effBonus=" << body.efficiencyBonus << "base=" << values.base
             << "FD=" << values.firstDiscovery << "FM=" << values.firstMapped
             << "Mapped=" << values.mapped;

    return values.base;
}

int ExplorationValue::calculateExobiologyValue(const CartographicBody &body) {
  return body.biologicalSignals * 500000;
}