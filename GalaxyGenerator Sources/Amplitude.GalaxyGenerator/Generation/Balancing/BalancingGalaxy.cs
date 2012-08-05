using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Amplitude.GalaxyGenerator.Generation.Components;

namespace Amplitude.GalaxyGenerator.Generation.Balancing
{
    public class BalancingGalaxy
    {
        public List<BalancingStarSystem> Stars { get; protected set; }
        public List<WarpLine> Warps { get; protected set; }
        public List<Constellation> Constellations { get; protected set; }
        public List<Region> Regions { get; protected set; }
        public List<BalancingStarSystem> SpawnStars { get; protected set; }
        public List<Planet> Planets { get; protected set; }
        private List<BalancingPlayer> Players;
        //This is for finding BalancingStarSystems when traversing over warpLanes.
        public Dictionary<StarSystem, BalancingStarSystem> StarCorrespondence { get; protected set; }

        //Open for discussion.
        private const double MAX_DISTANCE = 16.0;

        public BalancingGalaxy(Galaxy gal)
        {
            this.Stars = new List<BalancingStarSystem>();
            this.Warps = new List<WarpLine>();
            this.Constellations = new List<Constellation>();
            this.Regions = new List<Region>();
            this.SpawnStars = new List<BalancingStarSystem>();
            this.StarCorrespondence = new Dictionary<StarSystem,BalancingStarSystem>();
            Players = new List<BalancingPlayer>();

            gal.Stars.ForEach((star) => {
                BalancingStarSystem bstar = new BalancingStarSystem(star);
                Stars.Add(bstar);
                StarCorrespondence.Add(star, bstar);
            });
            gal.SpawnStars.ForEach((star) =>
            {
                BalancingStarSystem bstar = StarCorrespondence[star];
                Players.Add(new BalancingPlayer(bstar));
            });
            Warps = gal.Warps;
        }

        public List<BalancingPlayer> playerScores()
        {
            Dictionary<BalancingPlayer, HashSet<BalancingStarSystem>> playerSystems = new Dictionary<BalancingPlayer, HashSet<BalancingStarSystem>>();
            Players.ForEach((player) =>
            {
                player.Stars = findNearbyStars(MAX_DISTANCE, player);
                //playerSystems.Add(player, nearby);
            });
            Players.ForEach((player) =>
            {
                player.score = playerScore(player);
            });
            return Players;
        }

        //Find nearby Stars greedily
        private HashSet<BalancingStarSystem> findNearbyStars(double maxDistance, BalancingPlayer player)
        {
            BalancingStarSystem start = player.SpawnStar;

            //Create a list of stars in range of the player and also to prevent doubly adding stars.
            HashSet<BalancingStarSystem> starsInRange = new HashSet<BalancingStarSystem>();
            SortedDictionary<double,BalancingStarSystem> visitableStars = new SortedDictionary<double,BalancingStarSystem>();

            start.PlayerDistance.Add(player, 0.0);
            visitableStars.Add(0.0, start);
            starsInRange.Add(start);

            while (visitableStars.Count > 0)
            {
                KeyValuePair<double, BalancingStarSystem> pair = visitableStars.First();
                visitableStars.Remove(pair.Key);
                //Trace.WriteLine("Visiting distance: " + pair.Key);
                HashSet<StarSystem> connectedStars = pair.Value.attachedStarSystem.destinations;

                foreach(StarSystem star in connectedStars){
                    BalancingStarSystem bstar = StarCorrespondence[star];
                    double distance = star.directDistanceTable[start.attachedStarSystem];
                    //If the target system is too far ignore it.
                    //If the the system has already been added ignore it.
                    //If there is a wormhole between the two ignore this connection.
                    if (distance < maxDistance 
                        && !starsInRange.Contains(bstar) 
                        && !wormhole(star, pair.Value.attachedStarSystem))
                    {
                        bstar.PlayerDistance.Add(player, distance);
                        visitableStars.Add(distance, bstar);
                        starsInRange.Add(bstar);
                    }
                }         
            }

            return starsInRange;
        }

        private bool wormhole(StarSystem start, StarSystem end)
        {
            return Warps.Any((warp) => warp.isWormhole 
                && (warp.starA == start || warp.starA == end)
                && (warp.starB == start || warp.starB == end));
        }

        private double playerScore(BalancingPlayer player)
        {
            return player.Stars.Aggregate(0.0, (scoreAccumulator, system) => scoreAccumulator + system.getSystemWorth(player));
        }

    }
}
