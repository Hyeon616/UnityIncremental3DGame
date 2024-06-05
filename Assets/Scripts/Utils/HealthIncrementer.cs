public class HealthIncrementer
{
    private int incrementValue;
    private int currentIncrement;

    public HealthIncrementer(int incrementValue)
    {
        this.incrementValue = incrementValue;
        this.currentIncrement = 0;
    }

    public int GetNextHealth(int baseHealth)
    {
        int nextHealth = baseHealth + currentIncrement;
        currentIncrement += incrementValue;
        return nextHealth;
    }

    public void Reset()
    {
        currentIncrement = 0;
    }
}
