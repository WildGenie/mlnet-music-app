﻿using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IntelligentDemo.Models
{
    class EmotionService
    {
        private FaceAPI _faceAPI = new FaceAPI(new ApiKeyServiceClientCredentials(App.Secrets.EmotionKey));
        private IList<FaceAttributeType> _attributes = new[] { FaceAttributeType.Emotion };

        public async Task<string> DetectEmotionFromFile(string file)
        {
            try
            {
                using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var result = await _faceAPI.Face.DetectWithStreamAsync(stream, returnFaceAttributes: _attributes);

                    if (result.Any())
                    {
                        var emotion = result.First().FaceAttributes.Emotion;
                        return GetPrimaryEmotion(emotion);
                    }

                    return "No face detected";
                }
            }
            catch (Exception)
            {
                return "API error";
            }

        }

        public async Task<string> DetectEmotionFromUrl(string url)
        {
            try
            {
                var result = await _faceAPI.Face.DetectWithUrlAsync(url, returnFaceAttributes: _attributes);

                if (result.Any())
                {
                    var emotion = result.First().FaceAttributes.Emotion;
                    return GetPrimaryEmotion(emotion);
                }

                return "No face detected";
            }
            catch (Exception)
            {
                return "API error";
            }
        }

        private string GetPrimaryEmotion(Emotion emotion)
        {
            var emotions = new[]
            {
                new { Name = "Anger", Value = emotion.Anger },
                new { Name = "Contempt", Value = emotion.Contempt },
                new { Name = "Disgust", Value = emotion.Disgust },
                new { Name = "Fear", Value = emotion.Fear },
                new { Name = "Happiness", Value = emotion.Happiness },
                new { Name = "Neutral", Value = emotion.Neutral },
                new { Name = "Sadness", Value = emotion.Sadness },
                new { Name = "Surprise", Value = emotion.Surprise },
            };

            return emotions
                .OrderByDescending(e => e.Value)
                .First()
                .Name;
        }
    }
}
