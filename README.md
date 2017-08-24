# What?

* `UniRx.ObservableUnityWebRequest` を提供します。
* `UniRx.ObservableWWW` の `UnityWebRequest` 版です。

# Why?

* UnityWebRequest 版が無かったので。

# Install

```shell
$ npm install @umm/unirx_observableunitywebrequest
```

# Usage

```csharp
using UniRx;

public class Sample {

    void Start() {
        ObservableUnityWebRequest.GetText("https://www.google.com/").Subscribe(
            (responseText) => {
                Debug.Log(responseText);
            }
        );
    }

}
```

* `UnityWebRequest` が提供する主立ったメソッドをラップしています。

# License

Copyright (c) 2017 Tetsuya Mori

Released under the MIT license, see [LICENSE.txt](LICENSE.txt)

