using Akka.Actor;
using Akka.Cluster;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AkkaActor
{
    internal class ActorManager : ReceiveActor
    {
        protected Cluster Cluster = Cluster.Get(Context.System);
        private static ActorSelection AkkaActor1;
        static int i = 0;
        #region BOILERPLATING

        protected override void PreStart()
        {
            Cluster.Subscribe(Self, new[] { typeof(ClusterEvent.IMemberEvent) });
        }

        private void HandleOnReStart(Terminated terminated)
        {
            if (terminated.ExistenceConfirmed && AkkaActor1 != null) AkkaActor1.Tell("Restart");
        }


        protected override void PostStop()
        {
            Cluster.Unsubscribe(Self);
        }

        private void HandleMemberUp(ClusterEvent.IMemberEvent memberUp)
        {
            Context.Watch(Sender);

            if (memberUp.Member.HasRole("actor1") && AkkaActor1 == null) AkkaActor1 = Context.ActorSelection(memberUp.Member.Address + "/user/Actor1Manager");

        }

        #endregion BOILERPLATING
        public ActorManager()
        {
            Receive<string>(s => HandleStart(s));
            Receive<ClusterEvent.IMemberEvent>(m => HandleMemberUp(m));
            Receive<Terminated>(t => HandleOnReStart(t));

        }

        private void HandleStart(string message)
        {
            Console.WriteLine("I received - " + message);
            Context.Watch(Sender);
            if (AkkaActor1 != null)
            {
                Thread.Sleep(2000);

                AkkaActor1.Tell(i++);
            }
            else Self.Tell("starting again");
        }
    }
}
