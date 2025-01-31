public class Stats
{
    private int charisma;
    private int speed;
    private int luck;

    public Stats(int charisma, int speed, int luck)
    {
        this.charisma = charisma;
        this.speed = speed;
        this.luck = luck;
    }

    public int GetCharisma()
    {
        return charisma;
    }

    public int GetSpeed()
    {
        return speed;
    }

    public int GetLuck()
    {
        return luck;
    }
}
