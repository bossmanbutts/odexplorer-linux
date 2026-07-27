#include "EventFormatter.hpp"

QString EventFormatter::format(
    const QJsonObject &obj,
    const GameState &state)
{
    const QString event = obj["event"].toString();

    if (event == "LoadGame")
    {
        return QString("Logged in as %1")
            .arg(obj["Commander"].toString());
    }

    if (event == "Location")
    {
        return QString("Entered %1")
            .arg(obj["StarSystem"].toString());
    }

    if (event == "StartJump") 
    {
        QString jumpType = obj["JumpType"].toString();

        if (jumpType == "Supercruise")
            return "Entering supercruise";

        if (jumpType == "Hyperspace")
            return "Jumping to " + obj["StarSystem"].toString();
    }

    if (event == "FSDJump")
    {
        return QString("Arrived in %1")
            .arg(obj["StarSystem"].toString());
    }

    if (event == "SupercruiseEntry")
    {
        return "Entered supercruise";
    }

    if (event == "SupercruiseExit")
    {
        return QString("Exited supercruise near %1")
            .arg(obj["Body"].toString());
    }

    if (event == "Docked")
    {
        return QString("Docked at %1")
            .arg(obj["StationName"].toString());
    }

    if (event == "Undocked") {
      if (obj.contains("StationName"))
        return "Undocked from " + obj["StationName"].toString();

      if (!state.station.isEmpty())
        return "Undocked from " + state.station;

      return "Undocked";
    }

    if (event == "FuelScoop")
    {
        return "Fuel scooping...";
    }

    if (event == "ReservoirReplenished")
    {
        return "Fuel reservoir replenished.";
    }

    if (event == "FSSDiscoveryScan")
    {
        return "Discovery scan complete";
    }

    if (event == "Scan") {
      QString body = obj["BodyName"].toString();

      if (obj.contains("PlanetClass")) {
        QString planet = obj["PlanetClass"].toString();
        bool terraformable =
            obj["TerraformState"].toString() == "Terraformable";

        if (planet == "Earthlike body")
          return "Earth-like World: " + body;

        if (planet == "Water world")
          return terraformable ? "Terraformable Water World: " + body
                               : "Water World: " + body;

        if (planet == "Ammonia world")
          return "Ammonia World: " + body;

        if (planet == "High metal content body" && terraformable)
          return "Terraformable High Metal Content World: " + body;

        if (planet == "Rocky body" && terraformable)
          return "Terraformable Rocky World: " + body;

        if (terraformable)
          return "Terraformable planet: " + body;

        if (body.contains("Belt Cluster"))
          return {};

        return QString("Scanned %1 (%2)").arg(body).arg(planet);
      }

      if (obj.contains("StarType")) {
        QString star = obj["StarType"].toString();

        if (star == "N")
          return "Neutron Star: " + body;

        if (star == "H")
          return "Black Hole: " + body;

        return QString("Scanned %1 (%2 star)").arg(body).arg(star);
      }

      return "Scanned " + body;
    }

    if (event == "SAAScanComplete") {
      return QString("Mapped %1").arg(obj["BodyName"].toString());
    }

    if (event == "FSSAllBodiesFound") 
    {
        return "Completed system survey for " + state.system;
    }

    if (event == "FSSSignalDiscovered") 
    {
        QString signal = obj["SignalNameLocalised"].toString();

        if (signal.isEmpty())
            signal = obj["SignalName"].toString();

        if (signal.startsWith("$"))
            return {};

        if (!signal.isEmpty())
            return "Discovered signal: " + signal;

        return {};
    }

    if (event == "CodexEntry")
    {
        return QString("Codex entry: %1")
            .arg(obj["Name"].toString());
    }

    if (event == "DockingGranted") {
      return "Docking request granted";
    }

    if (event == "DockingDenied") {
      return "Docking request denied";
    }

    if (event == "Docked") {
      return "Docked at " + obj["StationName"].toString();
    }

    if (event == "Undocked") {
      return "Undocked from " + obj["StationName"].toString();
    }
    return {};
}