using System.Threading.Tasks;
using UnityEngine;

namespace Tools
{
    public static class AnimationExtension
    {
        public static AnimationClip GetAnimationClipByName(this Animator animator, string name)
        {
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == name)
                {
                    return clip;
                }
            }
            return null;
        }

        public static float GetClipSpeed(this Animator animator, string clipName)
        {
            AnimatorControllerParameter[] parameters = animator.parameters;
            foreach (AnimatorControllerParameter param in parameters)
            {
                if (param.type == AnimatorControllerParameterType.Float && param.name == clipName)
                {
                    return animator.GetFloat(param.name);
                }
            }

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(clipName))
            {
                return stateInfo.speed;
            }

            return 1f; // Default speed if no override is found
        }

        public static async Task ClipPlaying(this Animator animator, string clipName)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName(clipName))
            {
                float clipLength = GetAnimationClipLength(clipName);
                float elapsedTime = stateInfo.normalizedTime * clipLength;
                float remainingTime = Mathf.Max(0, clipLength - elapsedTime);

                Debug.Log($"Waiting for remaining {remainingTime} seconds...");
                await Task.Delay(Mathf.RoundToInt(remainingTime * 1000));
            }
            else
            {
                Debug.Log("Animation not playing.");
            }

            float GetAnimationClipLength(string clipName)
            {
                RuntimeAnimatorController controller = animator.runtimeAnimatorController;
                foreach (AnimationClip clip in controller.animationClips)
                {
                    if (clip.name == clipName)
                    {
                        return clip.length;
                    }
                }
                return 0f;
            }
        }

    }
}