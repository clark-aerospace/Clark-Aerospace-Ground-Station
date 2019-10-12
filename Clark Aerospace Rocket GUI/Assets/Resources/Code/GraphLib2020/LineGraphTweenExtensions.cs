using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GraphLib; 
using DG.Tweening;
using TMPro;


namespace GraphLib {
    public static class LineGraphTweenExtensions {
        public static void SetXMinMaxAnimated(this LineGraph graph, float min, float max, float animateDuration = 0.2f) {

            Debug.Log(string.Format("Setting x bounds to {0} - {1}", min, max));

            if (min > max) {
                Debug.LogError(string.Format("Error with SetXMinMaxAnimated: The minimum value {0} is greater than the maximum value, {1}.", min, max));
                return;
            }
            Sequence seq = DOTween.Sequence();

            seq = AddLabelFadeOutToTweenSequence(graph, seq, animateDuration);

            seq.Append(DOTween.To(() => graph.xMinimum, x => graph.xMinimum = x, min, animateDuration));
            seq.Join(DOTween.To(() => graph.xMaximum, x => graph.xMaximum = x, max, animateDuration));

            seq = AddLabelFadeInToTweenSequence(graph, seq, animateDuration);
        }

        public static void SetYMinMaxAnimated(this LineGraph graph, float min, float max, float animateDuration = 0.2f) {
            if (min > max) {
                Debug.LogError(string.Format("Error with SetYMinMaxAnimated: The minimum value {0} is greater than the maximum value, {1}.", min, max));
                return;
            }
            Sequence seq = DOTween.Sequence();
            seq = AddLabelFadeOutToTweenSequence(graph, seq, animateDuration);

            seq.Append(DOTween.To(() => graph.yMinimum, x => graph.yMinimum = x, min, animateDuration));
            seq.Join(DOTween.To(() => graph.yMaximum, x => graph.yMaximum = x, max, animateDuration));

            seq = AddLabelFadeInToTweenSequence(graph, seq, animateDuration);
        }

        public static void SetBoundsAnimated(this LineGraph graph, Vector2 xBounds, Vector2 yBounds, float animateDuration = 0.2f) {
            if (xBounds.x > xBounds.y) {
                Debug.LogError(string.Format("Error with SetBoundsAnimated: The minimum X value {0} is greater than the maximum X value, {1}.", xBounds.x, xBounds.y));
                return;
            }

            if (yBounds.x > yBounds.y) {
                Debug.LogError(string.Format("Error with SetBoundsAnimated: The minimum Y value {0} is greater than the maximum Y value, {1}.", yBounds.x, yBounds.y));
                return;
            }

            Sequence seq = DOTween.Sequence();

            seq = AddLabelFadeOutToTweenSequence(graph, seq, animateDuration);

            seq.Append(DOTween.To(() => graph.xMinimum, x => graph.xMinimum = x, xBounds.x, animateDuration));
            seq.Join(DOTween.To(() => graph.xMaximum, x => graph.xMaximum = x, xBounds.y, animateDuration));

            seq.Join(DOTween.To(() => graph.yMinimum, x => graph.yMinimum = x, yBounds.x, animateDuration));
            seq.Join(DOTween.To(() => graph.yMaximum, x => graph.yMaximum = x, yBounds.y, animateDuration));

            seq = AddLabelFadeInToTweenSequence(graph, seq, animateDuration);
        }



        private static Sequence AddLabelFadeOutToTweenSequence(LineGraph graph, Sequence seq, float dur) {
            seq.Append(DOTween.To(() => graph.GridAlpha, x => graph.GridAlpha = x, 0, dur/2));
            foreach (TextMeshProUGUI xLabel in graph.xAxisLabels) {
                seq.Join(DOTween.To(() => xLabel.alpha, x => xLabel.alpha = x, 0, dur/2));
            }

            foreach (TextMeshProUGUI yLabel in graph.yAxisLabels) {
                seq.Join(DOTween.To(() => yLabel.alpha, x => yLabel.alpha = x, 0, dur/2));
            }

            seq.AppendCallback(graph.ResetAxisLabels);
            return seq;
        }

        private static Sequence AddLabelFadeInToTweenSequence(LineGraph graph, Sequence seq, float dur) {
            seq.Append(DOTween.To(() => graph.GridAlpha, x => graph.GridAlpha = x, 1, dur/2));
            foreach (TextMeshProUGUI xLabel in graph.xAxisLabels) {
                seq.Join(DOTween.To(() => xLabel.alpha, x => xLabel.alpha = x, 1, dur/2));
            }

            foreach (TextMeshProUGUI yLabel in graph.yAxisLabels) {
                seq.Join(DOTween.To(() => yLabel.alpha, x => yLabel.alpha = x, 1, dur/2));
            }
            
            return seq;
        }
    }
}