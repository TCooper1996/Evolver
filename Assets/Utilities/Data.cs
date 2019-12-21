using System;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

public static class Data
{
    
    private static Normal _healthMultiplierDistribution = new Normal(1, 0.4, new Xoshiro256StarStar());
    private static Normal _attackTimeMultiplierDistribution = new Normal(0.9, 0.05, new Xoshiro256StarStar());
    private static Normal _damageMultiplierDistribution = new Normal(1, 0.2, new Xoshiro256StarStar());
    private static readonly Normal _bulletsAddDistribution = new Normal();

    public static double healthSample => _healthMultiplierDistribution.Sample();
    public static double attackTimeSample => _attackTimeMultiplierDistribution.Sample();
    public static double damageSample => Math.Max(1.05, _damageMultiplierDistribution.Sample());
    public static int bulletsSample => (int)Math.Round(_bulletsAddDistribution.Sample());
    
    public static void SetDepth(int depth)
    {
        _healthMultiplierDistribution = new Normal(Math.Pow(depth, 0.7), 0.4);
        _attackTimeMultiplierDistribution = new Normal(0.9, 0.05);
        _damageMultiplierDistribution = new Normal(Math.Pow(depth, 0.5), 0.2);
    }
    
}
