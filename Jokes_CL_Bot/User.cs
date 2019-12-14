using System;


namespace JokesBot
{
    class User
    {
        /// <summary>
        /// Stage of identification
        /// </summary>
        public int ident = 3;
        /// <summary>
        /// User's Telegram ID
        /// </summary>
        public long id = 0;
        /// <summary>
        /// The evening notify flag.
        /// </summary>
        public bool eveningNotify = false;
        /// <summary>
        /// The pre lesson notify flag.
        /// </summary>
        public bool morningNotify = false;
        /// <summary>
        /// Flag that user was notified today with preLessonNotifier
        /// </summary>
        public bool notifiedToday = false;
        /// <summary>
        /// The last access time.
        /// </summary>
        public DateTime LastAccess = new DateTime(2019,12,13,0,0,0); //presentation date

    }
}
