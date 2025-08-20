using UnityEngine;

public enum Team
{
  Player,
  Enemy
}

public static class TeamManager
{
  public static bool IsOpponent(Entity attacker, Entity target)
  {
    Team attackerTeam = attacker.team;
    Team targetTeam = target.team;

    return attackerTeam != targetTeam;
  }
}