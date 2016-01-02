/*
    CM3D2.ExAdvHooks -- An example to show off how to use Mono.Cecil to inject methods with parameters and memeber fields.
*/
using System;
using param;

namespace CM3D2.ExAdvHooks.Hook
{
    /// <summary>
    /// Class for our hooks.
    /// </summary>
    public static class Hooks
    {
        /// <summary>
        /// A hook that will be called every time Maid.CrossFade(string fn, bool additive, bool loop, bool addQueue, float val, float weight) method is called.
        /// 
        /// This hook will receive the instance of Maid in which the CrossFade was called and all the parameters of CrossFade.
        /// </summary>
        public static void CrossFadeHook(Maid m, string fn, bool additive, bool loop, bool addQueue, float val, float weight)
        {
            Console.WriteLine($"Crossfading on maid {m.Param.status.first_name} {m.Param.status.last_name}! Function: {fn}, Additive: {additive}, In loop: {loop}, Add to queue: {addQueue}, Value: {val}, Weight: {weight}");
        }

        /// <summary>
        /// A hook that will be called at the end of MaidParam.SetCurExcite(int value).
        /// 
        /// This method will receive field status_ found in MaidParam
        /// </summary>
        public static void SetCurExciteMax(Status status)
        {
            status.cur_excite = 300;
        }
    }
}
