using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UniRx {

    // ReSharper disable once PartialTypeWithSinglePart
    public static partial class ObservableUnityWebRequest {

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
            return Get(
                url,
                (DownloadHandlerTexture downloadHandler) => downloadHandler.texture,
                requestHeaderMap,
                progress
            );
        }

        public static IObservable<AudioClip> GetAudioClip(string url, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Get(
                url,
                (DownloadHandlerAudioClip downloadHandler) => downloadHandler.audioClip,
                requestHeaderMap,
                progress
            );
        }

#if !UNITY_IOS && !UNITY_ANDROID
        public static IObservable<MovieTexture> GetMovieTexture(string url, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Get(
                url,
                (DownloadHandlerMovieTexture downloadHandler) => downloadHandler.movieTexture,
                requestHeaderMap,
                progress
            );
        }
#endif

        public static IObservable<T> Get<T, TDownloadHandler>(string url, Func<TDownloadHandler, T> downloadedCallback, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) where TDownloadHandler : DownloadHandler {
            return Observable.FromCoroutine<T>(
                (observer, cancellationToken) => Request(
                    UnityWebRequest.Get(url),
                    requestHeaderMap,
                    downloadedCallback,
                    observer,
                    progress,
                    cancellationToken
                )
            );
        }

        public static IObservable<AssetBundle> GetAssetBundle(string url, uint crc, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return GetAssetBundle(
                () => UnityWebRequest.GetAssetBundle(url, crc),
                requestHeaderMap,
                progress
            );
        }

        public static IObservable<AssetBundle> GetAssetBundle(string url, uint version, uint crc, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return GetAssetBundle(
                () => UnityWebRequest.GetAssetBundle(url, version, crc),
                requestHeaderMap,
                progress
            );
        }

        public static IObservable<AssetBundle> GetAssetBundle(string url, Hash128 hash, uint crc, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return GetAssetBundle(
                () => UnityWebRequest.GetAssetBundle(url, hash, crc),
                requestHeaderMap,
                progress
            );
        }

        public static IObservable<AssetBundle> GetAssetBundle(string url, CachedAssetBundle cachedAssetBundle, uint crc, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return GetAssetBundle(
                () => UnityWebRequest.GetAssetBundle(url, cachedAssetBundle, crc),
                requestHeaderMap,
                progress
            );
        }

        public static IObservable<AssetBundle> GetAssetBundle(Func<UnityWebRequest> callback, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Observable.FromCoroutine<AssetBundle>(
                (observer, cancellationToken) => {
                    return Request(
                        callback(),
                        requestHeaderMap,
                        (DownloadHandlerAssetBundle downloadHandler) => downloadHandler.assetBundle,
                        observer,
                        progress,
                        cancellationToken
                    );
                }
            );
        }

        public static IObservable<Unit> Post(string url, Dictionary<string, string> postData, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            WWWForm form = new WWWForm();
            foreach (KeyValuePair<string, string> field in postData) {
                form.AddField(field.Key, field.Value);
            }
            return Post(url, form, requestHeaderMap, progress);
        }

        public static IObservable<Unit> Post(string url, WWWForm postData, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Observable.FromCoroutine<Unit>(
                (observer, cancellationToken) => Request(
                    UnityWebRequest.Post(url, postData),
                    requestHeaderMap,
                    (DownloadHandler _) => Unit.Default,
                    observer,
                    progress,
                    cancellationToken
                )
            );
        }

        public static IObservable<Unit> Post(string url, List<IMultipartFormSection> multipartFormSectionList, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Observable.FromCoroutine<Unit>(
                (observer, cancellationToken) => Request(
                    UnityWebRequest.Post(url, multipartFormSectionList),
                    requestHeaderMap,
                    (DownloadHandler _) => Unit.Default,
                    observer,
                    progress,
                    cancellationToken
                )
            );
        }

        public static IObservable<Unit> Post(string url, List<IMultipartFormSection> multipartFormSectionList, byte[] boundary, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Observable.FromCoroutine<Unit>(
                (observer, cancellationToken) => Request(
                    UnityWebRequest.Post(url, multipartFormSectionList, boundary),
                    requestHeaderMap,
                    (DownloadHandler _) => Unit.Default,
                    observer,
                    progress,
                    cancellationToken
                )
            );
        }

        public static IObservable<Unit> Post(string url, string postData, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Observable.FromCoroutine<Unit>(
                (observer, cancellationToken) => Request(
                    UnityWebRequest.Post(url, postData),
                    requestHeaderMap,
                    (DownloadHandler _) => Unit.Default,
                    observer,
                    progress,
                    cancellationToken
                )
            );
        }

        public static IObservable<Unit> Head(string url, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Observable.FromCoroutine<Unit>(
                (observer, cancellationToken) => Request(
                    UnityWebRequest.Head(url),
                    requestHeaderMap,
                    (DownloadHandler _) => Unit.Default,
                    observer,
                    progress,
                    cancellationToken
                )
            );
        }

        public static IObservable<Unit> Put(string url, string data, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Observable.FromCoroutine<Unit>(
                (observer, cancellationToken) => Request(
                    UnityWebRequest.Put(url, data),
                    requestHeaderMap,
                    (DownloadHandler _) => Unit.Default,
                    observer,
                    progress,
                    cancellationToken
                )
            );
        }

        public static IObservable<Unit> Put(string url, byte[] data, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Observable.FromCoroutine<Unit>(
                (observer, cancellationToken) => Request(
                    UnityWebRequest.Put(url, data),
                    requestHeaderMap,
                    (DownloadHandler _) => Unit.Default,
                    observer,
                    progress,
                    cancellationToken
                )
            );
        }

        public static IObservable<Unit> Delete(string url, Dictionary<string, string> requestHeaderMap = null, IProgress<float> progress = null) {
            return Observable.FromCoroutine<Unit>(
                (observer, cancellationToken) => Request(
                    UnityWebRequest.Delete(url),
                    requestHeaderMap,
                    (DownloadHandler _) => Unit.Default,
                    observer,
                    progress,
                    cancellationToken
                )
            );
        }

        private static IEnumerator Request<T, TDownloadHandler>(UnityWebRequest request, Dictionary<string, string> requestHeaderMap, Func<TDownloadHandler, T> downloadedCallback, IObserver<T> observer, IProgress<float> progress, CancellationToken cancellationToken) where TDownloadHandler : DownloadHandler {
            using (request) {
                request.SetRequestHeaders(requestHeaderMap);
                if (progress != default(IProgress<float>)) {
                    AsyncOperation operation = request.Send();
                    while (!operation.isDone && !cancellationToken.IsCancellationRequested) {
                        try {
                            progress.Report(operation.progress);
                        } catch (Exception e) {
                            observer.OnError(e);
                            yield break;
                        }
                        yield return null;
                    }
                } else {
                    yield return request.Send();
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
                if (downloadedCallback != default(Func<TDownloadHandler, T>) && (request.downloadHandler as TDownloadHandler) != default(TDownloadHandler)) {
                    observer.OnNext(downloadedCallback((TDownloadHandler)request.downloadHandler));
                }
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

            public System.Net.HttpStatusCode StatusCode {
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
                this.Request = request;
                this.RawErrorMessage = request.error;
                this.ResponseHeaders = request.GetResponseHeaders();
                this.HasResponse = false;

                this.StatusCode = (System.Net.HttpStatusCode)request.responseCode;

                if (request.downloadHandler != null && !(request.downloadHandler is DownloadHandlerAssetBundle)) {
                    this.Text = request.downloadHandler.text;
                }

                if (request.responseCode != 0) {
                    this.HasResponse = true;
                }
            }

            public override string ToString() {
                var text = this.Text;
                if (string.IsNullOrEmpty(text)) {
                    return RawErrorMessage;
                } else {
                    return RawErrorMessage + " " + text;
                }
            }

        }

    }

}
