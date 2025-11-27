namespace CursedOnion.Game.Systems.Level.Goal
{
    public class KillAllEnemiesGoal : LevelGoal
    {
        protected override void OnEnable()
        {
            LevelEvents.OnAllEnemiesKilled += OnVictory;
            LevelEvents.OnAllAlliesKilled += OnDefeat;
        }

        protected override void OnDisable()
        {
            LevelEvents.OnAllEnemiesKilled -= OnVictory;
            LevelEvents.OnAllAlliesKilled -= OnDefeat;
        }
    }
}