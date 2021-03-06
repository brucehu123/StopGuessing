﻿using System;
using System.Threading.Tasks;
using StopGuessing.Models;
using System.IO;

namespace Simulator
{
    
    public class Program
    {
        public static string BasePath = @"f:\OneDrive\StopGuessingData\";
        public static void Main(string[] args)
        {
            ulong sizeInMillions = 5;
            Simulator.RunExperimentalSweep(new[]
            {
                GetConfig(fractionOfMaliciousIPsToOverlapWithBenign:.01d, attack: ExperimentalConfiguration.AttackStrategy.BreadthFirst, pwToBan:100, scale: sizeInMillions),
                GetConfig(fractionOfMaliciousIPsToOverlapWithBenign:.01d, attack: ExperimentalConfiguration.AttackStrategy.BreadthFirst, pwToBan:0, scale: sizeInMillions),
                GetConfig(fractionOfMaliciousIPsToOverlapWithBenign:.01d, attack: ExperimentalConfiguration.AttackStrategy.BreadthFirst, pwToBan:10000, scale: sizeInMillions),
                GetConfig(fractionOfMaliciousIPsToOverlapWithBenign:.01d, attack: ExperimentalConfiguration.AttackStrategy.UseUntilLikelyPopular, pwToBan:100, scale: sizeInMillions),
                GetConfig(fractionOfMaliciousIPsToOverlapWithBenign:.01d, attack: ExperimentalConfiguration.AttackStrategy.Weighted, pwToBan:100, scale: sizeInMillions),
                GetConfig(fractionOfMaliciousIPsToOverlapWithBenign:.01d, addToName: "NoProxies", fractionOfBenignIPsBehindProxies:0d, attack:  ExperimentalConfiguration.AttackStrategy.BreadthFirst, pwToBan:100, scale: sizeInMillions),
                GetConfig(fractionOfMaliciousIPsToOverlapWithBenign:.01d, addToName: "Typosx5", extraTypoFactor: 5d ,attack: ExperimentalConfiguration.AttackStrategy.BreadthFirst, pwToBan:100, scale: sizeInMillions),
                GetConfig(fractionOfMaliciousIPsToOverlapWithBenign:.1d, attack: ExperimentalConfiguration.AttackStrategy.BreadthFirst, pwToBan:100, scale: sizeInMillions),
                GetConfig(fractionOfMaliciousIPsToOverlapWithBenign:.1d, attack: ExperimentalConfiguration.AttackStrategy.BreadthFirst, pwToBan:0, scale: sizeInMillions),
                GetConfig(fractionOfMaliciousIPsToOverlapWithBenign:.1d, attack: ExperimentalConfiguration.AttackStrategy.BreadthFirst, pwToBan:10000, scale: sizeInMillions),
                GetConfig(fractionOfMaliciousIPsToOverlapWithBenign:.1d, attack: ExperimentalConfiguration.AttackStrategy.UseUntilLikelyPopular, pwToBan:100, scale: sizeInMillions),
                GetConfig(fractionOfMaliciousIPsToOverlapWithBenign:.1d, attack: ExperimentalConfiguration.AttackStrategy.Weighted, pwToBan:100, scale: sizeInMillions),
                GetConfig(fractionOfMaliciousIPsToOverlapWithBenign:.1d, addToName: "NoProxies", fractionOfBenignIPsBehindProxies:0d, attack:  ExperimentalConfiguration.AttackStrategy.BreadthFirst, pwToBan:100, scale: sizeInMillions),
                GetConfig(fractionOfMaliciousIPsToOverlapWithBenign:.1d, addToName: "Typosx5", extraTypoFactor: 5d ,attack: ExperimentalConfiguration.AttackStrategy.BreadthFirst, pwToBan:100, scale: sizeInMillions),
                GetConfig(fractionOfMaliciousIPsToOverlapWithBenign:0d, addToName: "NoAttacks", fractionOfLoginAttemptsFromAttacker: 0d, attack:  ExperimentalConfiguration.AttackStrategy.BreadthFirst, pwToBan:100, scale: sizeInMillions),
                GetConfig(fractionOfMaliciousIPsToOverlapWithBenign:0d, addToName: "NoOverlapNoProxies", fractionOfBenignIPsBehindProxies:0d, attack:  ExperimentalConfiguration.AttackStrategy.BreadthFirst, pwToBan:100, scale: sizeInMillions),
                GetConfig(fractionOfMaliciousIPsToOverlapWithBenign:0d, addToName: "NoOverlap", attack:  ExperimentalConfiguration.AttackStrategy.BreadthFirst, pwToBan:100, scale: sizeInMillions),
                GetConfig(fractionOfMaliciousIPsToOverlapWithBenign:0d, addToName: "NoOverlapNoBannedPasswords", attack:  ExperimentalConfiguration.AttackStrategy.BreadthFirst, pwToBan:0, scale: sizeInMillions),
            });

        }

        public delegate void ConfigurationModifier(ref ExperimentalConfiguration confg);

        private const ulong Thousand = 1000;
        private const ulong Million = Thousand * Thousand;
        private const ulong Billion = Thousand * Million;

        public static ExperimentalConfiguration GetConfig(
            ExperimentalConfiguration.AttackStrategy attack = ExperimentalConfiguration.AttackStrategy.BreadthFirst,
            int pwToBan = 100,
            double fractionOfBenignIPsBehindProxies = 0.1,
            double fractionOfMaliciousIPsToOverlapWithBenign = .1d,
            double fractionOfLoginAttemptsFromAttacker = 0.5d,
            double extraTypoFactor = 1d,
            ulong scale = 1,
            string addToName = null)
        {
            ExperimentalConfiguration config = new ExperimentalConfiguration();
            // Scale of test
            config.AttackersStrategy = attack;
            config.PopularPasswordsToRemoveFromDistribution = pwToBan;
            config.FractionOfBenignIPsBehindProxies = fractionOfBenignIPsBehindProxies;
            config.FractionOfMaliciousIPsToOverlapWithBenign = fractionOfMaliciousIPsToOverlapWithBenign;

            ulong totalLoginAttempts = scale * Million;
            config.TestTimeSpan = new TimeSpan(7, 0, 0, 0); // 7 days
            double meanNumberOfLoginsPerBenignAccountDuringExperiment = 100d;
            double meanNumberOfLoginsPerAttackerControlledIP = 100d;

            DateTime now = DateTime.Now;
            string dirName = BasePath + "Run_" + totalLoginAttempts + "_" + now.Month + "_" + now.Day + "_" + now.Hour + "_" + now.Minute;
            Directory.CreateDirectory(dirName);
            config.OutputPath = dirName + @"\";

            config.OutputDirectoryName = string.Format("{0}_Strategy_{1}_Remove_{2}_Proxies_{3}_Overlap_{4}",
                addToName == null ? "" : addToName + "_",
                //(int)Math.Log10(totalLoginAttempts),
                config.AttackersStrategy == ExperimentalConfiguration.AttackStrategy.BreadthFirst
                    ? "BreadthFirst"
                    : config.AttackersStrategy == ExperimentalConfiguration.AttackStrategy.Weighted
                        ? "Weighted"
                        : "Avoid",
                config.PopularPasswordsToRemoveFromDistribution,
                (int)1000 * config.FractionOfBenignIPsBehindProxies,
                (int)1000 * config.FractionOfMaliciousIPsToOverlapWithBenign
                );

            // Figure out parameters from scale
            double fractionOfLoginAttemptsFromBenign = 1d - fractionOfLoginAttemptsFromAttacker;

            double expectedNumberOfBenignAttempts = totalLoginAttempts * fractionOfLoginAttemptsFromBenign;
            double numberOfBenignAccounts = expectedNumberOfBenignAttempts /
                                            meanNumberOfLoginsPerBenignAccountDuringExperiment;

            double expectedNumberOfAttackAttempts = totalLoginAttempts * fractionOfLoginAttemptsFromAttacker;
            double numberOfAttackerIps = expectedNumberOfAttackAttempts /
                                         meanNumberOfLoginsPerAttackerControlledIP;

            // Make any changes to the config or the config.BlockingOptions within config here
            config.TotalLoginAttemptsToIssue = totalLoginAttempts;

            config.FractionOfLoginAttemptsFromAttacker = fractionOfLoginAttemptsFromAttacker;
            config.NumberOfBenignAccounts = (uint)numberOfBenignAccounts;

            // Scale of attackers resources
            config.NumberOfIpAddressesControlledByAttacker = (uint)numberOfAttackerIps;
            config.NumberOfAttackerControlledAccounts = (uint)numberOfAttackerIps;

            // Additional sources of false positives/negatives
            config.ProxySizeInUniqueClientIPs = 1000;

            // Make typos almost entirely ignored
            config.ChanceOfBenignPasswordTypo *= extraTypoFactor;

            // Blocking parameters
            config.BlockingOptions.HeightOfBinomialLadder_H = 48;
            config.BlockingOptions.NumberOfBitsInBinomialLadderFilter_N = 1 << 29;
            config.BlockingOptions.BinomialLadderFrequencyThreshdold_T = 44;
            config.BlockingOptions.ExpensiveHashingFunctionIterations = 1;
            return config;
        }
    }
}
