using Akka.Actor;
using Akka.Cluster;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AkkaActor1
{
    internal class Actor1Manager : ReceiveActor
    {
        protected Cluster Cluster = Cluster.Get(Context.System);
        private static ActorSelection AkkaActor;

        #region BOILERPLATING

        protected override void PreStart()
        {
            Cluster.Subscribe(Self, new[] { typeof(ClusterEvent.IMemberEvent) });
        }

        private void HandleOnReStart(Terminated terminated)
        {
            if (terminated.ExistenceConfirmed) Self.Tell(string.Empty);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            Self.Tell(new Terminated(Self, true, false));
        }

        protected override void PostStop()
        {
            Cluster.Unsubscribe(Self);
        }

        private void HandleMemberUp(ClusterEvent.IMemberEvent memberUp)
        {
            if (memberUp.Member.HasRole("actor") && AkkaActor == null) AkkaActor = Context.ActorSelection(memberUp.Member.Address + "/user/ActorManager");

        }

        #endregion BOILERPLATING
        public Actor1Manager()
        {
            Receive<string>(s => HandleStart(s));
            Receive<int>(i => HandleResponse(i));
            Receive<ClusterEvent.IMemberEvent>(m => HandleMemberUp(m));
            Receive<Terminated>(t => HandleOnReStart(t));


        }

        private void HandleStart(string message)
        {
            Context.Watch(Sender);
            if (AkkaActor != null)
            {
                AkkaActor.Tell("Receiver Started!!!");
            }
            else Self.Tell("Receiver not reayd. Trying again!!!");
        }

        private void HandleResponse(int i)
        {
            Context.Watch(Sender);

            Console.WriteLine("I Sent Message " + i);
            Thread.Sleep(1000);

            AkkaActor.Tell("Message " + i);
        }
    }
}
