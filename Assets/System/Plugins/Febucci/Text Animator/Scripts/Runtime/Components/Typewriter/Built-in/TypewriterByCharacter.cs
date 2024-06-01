using Febucci.UI.Core;
using UnityEngine;

namespace Febucci.UI
{
    /// <summary>
    /// Built-in typewriter, which shows letters dynamically character after character.<br/>
    /// To enable it, add this component near a <see cref="TAnimCore"/> one<br/>
    /// - Base class: <see cref="TypewriterCore"/><br/>
    /// - Manual: <see href="https://www.febucci.com/text-animator-unity/docs/typewriters/">TextAnimatorPlayers</see>
    /// </summary>
    [HelpURL("https://www.febucci.com/text-animator-unity/docs/typewriters/")]
    [AddComponentMenu("Febucci/TextAnimator/Typewriter - By Character")]
    public class TypewriterByCharacter : Core.TypewriterCore
    {
        [Attributes.CharsDisplayTime, Tooltip("Wait time for normal letters")] public float waitForNormalChars = .03f;
        [Attributes.CharsDisplayTime, Tooltip("Wait time for ! ? .")] public float waitLong = .6f;
        [Attributes.CharsDisplayTime, Tooltip("Wait time for ; : ) - ,")] public float waitMiddle = .2f;
        [Tooltip("-True: only the last punctuaction on a sequence waits for its category time.\n-False: each punctuaction will wait, regardless if it's in a sequence or not")] public bool avoidMultiplePunctuactionWait = false;

        [Tooltip("True if you want the typewriter to wait for new line characters")] public bool waitForNewLines = true;

        [Tooltip("True if you want the typewriter to wait for all characters, false if you want to skip waiting for the last one")] public bool waitForLastCharacter = true;

        [Tooltip("True if you want to use the same typewriter's wait times for the disappearance progression, false if you want to use a different wait time")] public bool useTypewriterWaitForDisappearances = true;
        [Attributes.CharsDisplayTime, Tooltip("Wait time for characters in the disappearance progression")] public float disappearanceWaitTime = .015f;
        [Attributes.MinValue(0.1f), Tooltip("How much faster/slower is the disappearance progression compared to the typewriter's typing speed")] public float disappearanceSpeedMultiplier = 1;

        protected override float GetWaitAppearanceTimeOf(int charIndex)
        {
            char character = TextAnimator.Characters[charIndex].info.character;

            //avoids waiting for the last character
            if (!waitForLastCharacter && TextAnimator.allLettersShown)
                return 0;

            //avoids waiting for multiple times if there are puntuactions near each other
            if (avoidMultiplePunctuactionWait && char.IsPunctuation(character)) //curr char is punctuation
            {
                //next char is punctuation too, so skips this one
                if (charIndex < TextAnimator.CharactersCount - 1
                    && char.IsPunctuation(TextAnimator.Characters[charIndex + 1].info
                        .character))
                {
                    return waitForNormalChars;
                }
            }

            //avoids waiting for new lines
            if (!waitForNewLines && !TextAnimator.latestCharacterShown.info.isRendered)
            {
                static bool IsUnicodeNewLine(ulong unicode) //Returns true if the unicode value represents a new line
                {
                    return unicode == 10 || unicode == 13;
                }

                //skips waiting for a new line
                if (IsUnicodeNewLine(System.Convert.ToUInt64(TextAnimator.latestCharacterShown.info.character)))
                    return 0; //TODO test
            }

            //character is not before another punctuaction
            return character switch
            {
                ';' or ':' or ')' or '-' or ',' => waitMiddle,
                '!' or '?' or '.' => waitLong,
                _ => waitForNormalChars,
            };
        }

        protected override float GetWaitDisappearanceTimeOf(int charIndex) => useTypewriterWaitForDisappearances ? GetWaitAppearanceTimeOf(charIndex) * (1 / disappearanceSpeedMultiplier) : disappearanceWaitTime;
    }
}