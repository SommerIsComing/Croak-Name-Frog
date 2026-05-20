using System;
using UnityEngine;

// Klasse der indeholder events relateret til spillet, som kan bruges til at kommunikere mellem forskellige scripts og systemer i spillet
public static class GameEvent
{
    // Event der bliver kaldt når en animation skal afspilles, og sender navnet på den animation der skal afspilles som parameter
    public static Action<string> OnAnimNeeded;
}
