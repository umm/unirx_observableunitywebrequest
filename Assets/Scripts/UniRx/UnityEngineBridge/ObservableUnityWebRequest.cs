using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace UniRx {

    [PublicAPI]
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
    public static partial class ObservableUnityWebRequest {

        public static IObservable<UnityWebRequest> GetUnityWebRequest(string url, Func<string, UnityWebRequest> request = null, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Request(
                request == null ? UnityWebRequest.Get(url) : request(url),
                requestHeaderMap,
                progress
            );
        }

        public static IObservable<string> GetText(string url, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Get(
                url,
                (DownloadHandler downloadHandler) => downloadHandler.text,
                requestHeaderMap,
                progress
            );
        }

        public static IObservable<byte[]> GetData(string url, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Get(
                url,
                (DownloadHandler downloadHandler) => downloadHandler.data,
                requestHeaderMap,
                progress
            );
        }

        public static IObservable<Texture2D> GetTexture(string url, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Request(
                UnityWebRequestTexture.GetTexture(url),
                requestHeaderMap,
                (DownloadHandlerTexture downloadHandler) => downloadHandler.texture,
                progress
            );
        }

        public static IObservable<AudioClip> GetAudioClip(string url, AudioType audioType, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Request(
                UnityWebRequestMultimedia.GetAudioClip(url, audioType),
                requestHeaderMap,
                (DownloadHandlerAudioClip downloadHandler) => downloadHandler.audioClip,
                progress
            );
        }

#if !UNITY_IOS && !UNITY_ANDROID && !UNITY_2018_2_OR_NEWER
        public static IObservable<MovieTexture> GetMovieTexture(string url, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Request(
                UnityWebRequestMultimedia.GetMovieTexture(url),
                requestHeaderMap,
                (DownloadHandlerMovieTexture downloadHandler) => downloadHandler.movieTexture,
                progress
            );
        }
#endif

        public static IObservable<T> Get<T, TDownloadHandler>(string url, Func<TDownloadHandler, T> downloadedCallback, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) where TDownloadHandler : DownloadHandler {
            return Request(
                UnityWebRequest.Get(url),
                requestHeaderMap,
                downloadedCallback,
                progress
            );
        }

        public static IObservable<AssetBundle> GetAssetBundle(string url, uint crc, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return GetAssetBundle(
#if UNITY_2018_1_OR_NEWER
                () => UnityWebRequestAssetBundle.GetAssetBundle(url, crc),
#else
                () => UnityWebRequest.GetAssetBundle(url, crc),
#endif
                requestHeaderMap,
                progress
            );
        }

        public static IObservable<AssetBundle> GetAssetBundle(string url, uint version, uint crc, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return GetAssetBundle(
#if UNITY_2018_1_OR_NEWER
                () => UnityWebRequestAssetBundle.GetAssetBundle(url, version, crc),
#else
                () => UnityWebRequest.GetAssetBundle(url, version, crc),
#endif
                requestHeaderMap,
                progress
            );
        }

        public static IObservable<AssetBundle> GetAssetBundle(string url, Hash128 hash, uint crc, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return GetAssetBundle(
#if UNITY_2018_1_OR_NEWER
                () => UnityWebRequestAssetBundle.GetAssetBundle(url, hash, crc),
#else
                () => UnityWebRequest.GetAssetBundle(url, hash, crc),
#endif
                requestHeaderMap,
                progress
            );
        }

        public static IObservable<AssetBundle> GetAssetBundle(string url, CachedAssetBundle cachedAssetBundle, uint crc, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return GetAssetBundle(
#if UNITY_2018_1_OR_NEWER
                () => UnityWebRequestAssetBundle.GetAssetBundle(url, cachedAssetBundle, crc),
#else
                () => UnityWebRequest.GetAssetBundle(url, cachedAssetBundle, crc),
#endif
                requestHeaderMap,
                progress
            );
        }

        public static IObservable<AssetBundle> GetAssetBundle(Func<UnityWebRequest> callback, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            IObservable<AssetBundle> stream = Request(
                callback(),
                requestHeaderMap,
                (DownloadHandlerAssetBundle downloadHandler) => downloadHandler.assetBundle,
                progress
            );
            if (!Caching.ready) {
                Observable.EveryUpdate()
                    .Where(_ => Caching.ready)
                    .Take(1)
                    .SelectMany(_ => stream);
            }
            return stream;
        }

        public static IObservable<Unit> Post(string url, Dictionary<string, string> postData, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            WWWForm form = new WWWForm();
            foreach (KeyValuePair<string, string> field in postData) {
                form.AddField(field.Key, field.Value);
            }
            return Post(url, form, requestHeaderMap, progress);
        }

        public static IObservable<Unit> Post(string url, WWWForm postData, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Request(
                UnityWebRequest.Post(url, postData),
                requestHeaderMap,
                (DownloadHandler _) => Unit.Default,
                progress
            );
        }

        public static IObservable<Unit> Post(string url, List<IMultipartFormSection> multipartFormSectionList, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Request(
                UnityWebRequest.Post(url, multipartFormSectionList),
                requestHeaderMap,
                (DownloadHandler _) => Unit.Default,
                progress
            );
        }

        public static IObservable<Unit> Post(string url, List<IMultipartFormSection> multipartFormSectionList, byte[] boundary, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Request(
                UnityWebRequest.Post(url, multipartFormSectionList, boundary),
                requestHeaderMap,
                (DownloadHandler _) => Unit.Default,
                progress
            );
        }

        public static IObservable<Unit> Post(string url, string postData, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Request(
                UnityWebRequest.Post(url, postData),
                requestHeaderMap,
                (DownloadHandler _) => Unit.Default,
                progress
            );
        }

        public static IObservable<Unit> Head(string url, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Request(
                UnityWebRequest.Head(url),
                requestHeaderMap,
                (DownloadHandler _) => Unit.Default,
                progress
            );
        }

        public static IObservable<Unit> Put(string url, string data, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Request(
                UnityWebRequest.Put(url, data),
                requestHeaderMap,
                (DownloadHandler _) => Unit.Default,
                progress
            );
        }

        public static IObservable<Unit> Put(string url, byte[] data, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Request(
                UnityWebRequest.Put(url, data),
                requestHeaderMap,
                (DownloadHandler _) => Unit.Default,
                progress
            );
        }

        public static IObservable<Unit> Delete(string url, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Request(
                UnityWebRequest.Delete(url),
                requestHeaderMap,
                (DownloadHandler _) => Unit.Default,
                progress
            );
        }

        private static IObservable<UnityWebRequest> Request(UnityWebRequest request, Dictionary<string, string> requestHeaderMap, IProgress<float> progress) {
            return Observable
                .FromCoroutine<UnityWebRequest>(
                    (observer, cancellationToken) => SendRequest(
                        request,
                        requestHeaderMap,
                        observer,
                        progress,
                        cancellationToken
                    )
                );
        }

        private static IObservable<T> Request<T, TDownloadHandler>(UnityWebRequest request, Dictionary<string, string> requestHeaderMap, Func<TDownloadHandler, T> downloadedCallback, IProgress<float> progress) where TDownloadHandler : DownloadHandler {
            return Request(
                    request,
                    requestHeaderMap,
                    progress
                )
                .Select(
                    (unityWebRequest) => {
                        if (downloadedCallback != default(Func<TDownloadHandler, T>) && (request.downloadHandler as TDownloadHandler) != default(TDownloadHandler)) {
                            return downloadedCallback((TDownloadHandler)unityWebRequest.downloadHandler);
                        }
                        return default(T);
                    }
                );
        }

        private static IEnumerator SendRequest(UnityWebRequest request, Dictionary<string, string> requestHeaderMap, IObserver<UnityWebRequest> observer, IProgress<float> progress, CancellationToken cancellationToken) {
            using (request) {
                request.SetRequestHeaders(requestHeaderMap);
#if UNITY_2017_2_OR_NEWER
                AsyncOperation operation = request.SendWebRequest();
#else
                AsyncOperation operation = request.Send();
#endif
                while (!operation.isDone && !cancellationToken.IsCancellationRequested) {
                    if (progress != default(IProgress<float>)) {
                        try {
                            progress.Report(operation.progress);
                        } catch (Exception e) {
                            observer.OnError(e);
                            yield break;
                        }
                    }
                    yield return null;
                }

                if (cancellationToken.IsCancellationRequested) {
                    yield break;
                }

                if (progress != default(IProgress<float>)) {
                    try {
                        progress.Report(request.downloadProgress);
                    } catch (Exception e) {
                        observer.OnError(e);
                        yield break;
                    }
                }

                if (cancellationToken.IsCancellationRequested) {
                    yield break;
                }

                if (!string.IsNullOrEmpty(request.error)) {
                    observer.OnError(new UnityWebRequestErrorException(request));
                    yield break;
                }
                observer.OnNext(request);
                observer.OnCompleted();
            }
        }

        private static void SetRequestHeaders(this UnityWebRequest request, Dictionary<string, string> requestHeaderMap) {
            if (requestHeaderMap == default(Dictionary<string, string>)) {
                return;
            }
            foreach (KeyValuePair<string, string> requestHeader in requestHeaderMap) {
                request.SetRequestHeader(requestHeader.Key, requestHeader.Value);
            }
        }

        [PublicAPI]
        public class UnityWebRequestErrorException : Exception {

            public string RawErrorMessage {
                get;
                private set;
            }

            public bool HasResponse {
                get;
                private set;
            }

            public string Text {
                get;
                private set;
            }

            public HttpStatusCode StatusCode {
                get;
                private set;
            }

            public Dictionary<string, string> ResponseHeaders {
                get;
                private set;
            }

            public UnityWebRequest Request {
                get;
                private set;
            }

            // cache the text because if www was disposed, can't access it.
            public UnityWebRequestErrorException(UnityWebRequest request) {
                Request = request;
                RawErrorMessage = request.error;
                ResponseHeaders = request.GetResponseHeaders();
                HasResponse = false;

                StatusCode = (HttpStatusCode)request.responseCode;

                if (request.downloadHandler != null && !(request.downloadHandler is DownloadHandlerAssetBundle)) {
                    Text = request.downloadHandler.text;
                }

                if (request.responseCode != 0) {
                    HasResponse = true;
                }
            }

            public override string ToString() {
                var text = Text;
                if (string.IsNullOrEmpty(text)) {
                    return RawErrorMessage;
                }

                return RawErrorMessage + " " + text;
            }

        }

    }

}
