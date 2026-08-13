using System;
using System.Collections.Generic;
using EliteJournalReader;
using EliteJournalReader.Events;

namespace ODUtils.Exobiology;

public readonly struct ExoPlanet
{
	private readonly List<ShipMaterials>? _materials;

	private readonly long _systemAddress;

	public PlanetClass PlanetClass { get; }

	public AtmosphereDescription Atmosphere { get; }

	public AtmosphereClass AtmosphereClass { get; }

	public List<ScanItemComponent> AtmosphereComposition { get; }

	public Volcanism Volcanism { get; }

	public double SurfaceGravity { get; }

	public double SurfaceTemperature { get; }

	public double SurfacePressure { get; }

	public double DistanceFromArrivalLs { get; }

	public double OrbitalPeriod { get; }

	public List<ShipMaterials>? Materials
	{
		get
		{
			_materials?.Sort((ShipMaterials x, ShipMaterials y) => x.Percent.CompareTo(y.Percent));
			return _materials;
		}
	}

	public List<StarType> StarsInSystem { get; }

	public List<StarType> ParentStars { get; }

	public GalacticRegions Region { get; }

	public DateTime Timestamp { get; }

	public double DistanceToNebula { get; }

	public int BiologicalCount { get; }

	public long SystemAddress => _systemAddress;

	public ExoPlanet(PlanetClass planetClass, AtmosphereDescription atmosphere, AtmosphereClass atmosphereClass, List<ScanItemComponent> atmosphereComposition, Volcanism volcanism, double surfaceGravity, double surfaceTemperature, double surfacePressure, double distanceFromArrival, double orbitalPeriod, List<ShipMaterials>? materials, List<StarType> starsInSystem, List<StarType> parentStars, GalacticRegions region, DateTime timestamp, double distance, int biologicalCount, long systemAddress)
	{
		_materials = materials;
		_systemAddress = systemAddress;
		PlanetClass = planetClass;
		Atmosphere = atmosphere;
		AtmosphereClass = atmosphereClass;
		AtmosphereComposition = atmosphereComposition;
		Volcanism = volcanism;
		SurfaceGravity = surfaceGravity;
		SurfaceTemperature = surfaceTemperature;
		SurfacePressure = surfacePressure;
		DistanceFromArrivalLs = distanceFromArrival;
		OrbitalPeriod = orbitalPeriod;
		StarsInSystem = starsInSystem;
		ParentStars = parentStars;
		Region = region;
		Timestamp = timestamp;
		DistanceToNebula = distance;
		BiologicalCount = biologicalCount;
	}

	internal bool ContainsMaterial(PlanetMaterial material)
	{
		if (Materials == null)
		{
			return false;
		}
		foreach (ShipMaterials material2 in Materials)
		{
			if (material2.Name == material)
			{
				return true;
			}
		}
		return false;
	}

	internal bool ContainsStar(StarType starType)
	{
		foreach (StarType item in StarsInSystem)
		{
			if (item == starType)
			{
				return true;
			}
		}
		return false;
	}

	internal bool ContainsStars(List<StarType> starType)
	{
		foreach (StarType item in StarsInSystem)
		{
			if (starType.Contains(item))
			{
				return true;
			}
		}
		return false;
	}

	internal bool ContainsParentsStars(List<StarType> starType)
	{
		foreach (StarType parentStar in ParentStars)
		{
			if (starType.Contains(parentStar))
			{
				return true;
			}
		}
		return false;
	}
}
