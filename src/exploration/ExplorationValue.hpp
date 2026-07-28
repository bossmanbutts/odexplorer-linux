#pragma once

#include "../model/CartographicBody.hpp"

class ExplorationValue {
public:
  static int calculateBodyValue(const CartographicBody &body);
  static int calculateExobiologyValue(const CartographicBody &body);

private:
  struct EstimatedValues {
    int base = 0;
    int firstDiscovery = 0;
    int firstDiscoveryFirstMapped = 0;
    int firstDiscoveryFirstMappedEfficiently = 0;
    int firstMapped = 0;
    int firstMappedEfficiently = 0;
    int mapped = 0;
    int mappedEfficiently = 0;
  };

  static EstimatedValues calculateEstimatedValues(const CartographicBody &body);

  static double odysseyValue(double value);
  static double starValue33(double k, double mass);
  static double planetValue33(double k, double mass);
  static double planetValue32(double k, double mass);
};