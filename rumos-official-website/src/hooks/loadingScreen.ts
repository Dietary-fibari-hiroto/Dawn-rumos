export interface LoadingScreenOptions {
  loadingScreenId?: string;
  mainContentId?: string;
  minDisplayTime?: number;
  fadeOutDuration?: number;
  onComplete?: () => void;
}

export const initLoadingScreen = (options: LoadingScreenOptions = {}) => {
  const {
    loadingScreenId = "loading-screen",
    mainContentId = "main-content",
    minDisplayTime = 500,
    fadeOutDuration = 500,
    onComplete,
  } = options;

  const loadingScreen = document.getElementById(loadingScreenId);
  const mainContent = document.getElementById(mainContentId);

  if (!loadingScreen) return;

  const startTime = performance.now();

  const hideLoading = () => {
    const elapsedTime = performance.now() - startTime;
    const remainingTime = Math.max(0, minDisplayTime - elapsedTime);

    setTimeout(() => {
      loadingScreen.classList.add("hidden");
      mainContent?.classList.add("visible");

      setTimeout(() => {
        loadingScreen.remove();
        onComplete?.();
      }, fadeOutDuration);
    }, remainingTime);
  };

  if (document.readyState === "complete") {
    hideLoading();
  } else {
    window.addEventListener("load", hideLoading);
  }
};

//プログレスバー付きを書いてみる
export interface LoadingScreenWithProgressOptions extends LoadingScreenOptions {
  progressBarId?: string;
  progressTextId?: string;
  trackImages?: boolean; //画像の読み込みを追跡するか
  trackResources?: boolean; //その他のリソースも追跡するか
}

export const initLoadingScreenWithProgress = (
  options: LoadingScreenWithProgressOptions = {}
) => {
  const {
    loadingScreenId = "loading-screen",
    mainContentId = "main-content",
    progressBarId = "progress-bar",
    progressTextId = "progress-text",
    minDisplayTime = 500,
    fadeOutDuration = 500,
    trackImages = true,
    trackResources = false,
    onComplete,
  } = options;

  const loadingScreen = document.getElementById(loadingScreenId);
  const mainContent = document.getElementById(mainContentId);
  const progressBar = document.getElementById(progressBarId) as HTMLElement;
  const progressText = document.getElementById(progressTextId) as HTMLElement;

  if (!loadingScreen) return;

  let totalResources = 0;
  let loadedResources = 0;
  const startTime = performance.now();

  const updateProgress = () => {
    const progress =
      totalResources > 0
        ? Math.round((loadedResources / totalResources) * 100)
        : 0;

    if (progressBar) {
      progressBar.style.width = `${progress}%`;
    }
    if (progressText) {
      progressText.textContent = `${progress}%`;
    }

    return progress;
  };

  const hideLoading = () => {
    const elapsedTime = performance.now() - startTime;
    const remainingTime = Math.max(0, minDisplayTime - elapsedTime);

    setTimeout(() => {
      loadingScreen.classList.add("hidden");
      mainContent?.classList.add("visible");

      setTimeout(() => {
        loadingScreen.remove();
        onComplete?.();
      }, fadeOutDuration);
    }, remainingTime);
  };

  //画像の読み込みを追跡
  if (trackImages) {
    const images = Array.from(document.images);
    totalResources = images.length;

    if (totalResources === 0) {
      hideLoading();
      return;
    }

    images.forEach((img) => {
      if (img.complete) {
        loadedResources++;
      } else {
        img.addEventListener("load", () => {
          loadedResources++;
          const progress = updateProgress();
          if (progress === 100) {
            hideLoading();
          }
        });
        img.addEventListener("error", () => {
          loadedResources++;
          const progress = updateProgress();
          if (progress === 100) {
            hideLoading();
          }
        });
      }
    });

    //初期プログレスを更新
    const initialProgress = updateProgress();
    if (initialProgress === 100) {
      hideLoading();
    }
  } else {
    //通常のload待機
    if (document.readyState === "complete") {
      hideLoading();
    } else {
      window.addEventListener("load", hideLoading);
    }
  }
};

//プログレスバー付き+段階的フェード版
export interface LoadingScreenStageOptions
  extends LoadingScreenWithProgressOptions {
  loaderSelector?: string; //ローダー要素のセレクタ
  secondProcessSelector?: string; //2段階目の要素のセレクタ
  loaderFadeDuration?: number; //ローダーのフェード時間
  secondProcessFadeDuration?: number; // 2段階目のフェード時間
  secondProcessDisplayTime?: number; // 2段階目の表示時間
  finalFadeDuration?: number; // 最終フェードアウト時間
}

export const initLoadingScreenWithStages = (
  options: LoadingScreenStageOptions = {}
) => {
  const {
    loadingScreenId = "loading-screen",
    mainContentId = "main-content",
    progressBarId = "progress-bar",
    progressTextId = "progress-text",
    loaderSelector = ".loader",
    secondProcessSelector = ".second-proccess",
    minDisplayTime = 500,
    loaderFadeDuration = 500,
    secondProcessFadeDuration = 800,
    secondProcessDisplayTime = 2000,
    finalFadeDuration = 1000,
    trackImages = true,
    onComplete,
  } = options;

  const loadingScreen = document.getElementById(loadingScreenId);
  const mainContent = document.getElementById(mainContentId);
  const progressBar = document.getElementById(progressBarId) as HTMLElement;
  const progressText = document.getElementById(progressTextId) as HTMLElement;
  const loader = document.querySelector(loaderSelector) as HTMLElement;
  const secondProcess = document.querySelector(
    secondProcessSelector
  ) as HTMLElement;

  if (!loadingScreen) return;

  //2段階目を最初は非表示に
  if (secondProcess) {
    secondProcess.style.opacity = "0";
    secondProcess.style.pointerEvents = "none";
  }

  let totalResources = 0;
  let loadedResources = 0;
  const startTime = performance.now();

  const updateProgress = () => {
    const progress =
      totalResources > 0
        ? Math.round((loadedResources / totalResources) * 100)
        : 0;

    if (progressBar) {
      progressBar.style.width = `${progress}%`;
    }
    if (progressText) {
      progressText.textContent = `${progress}%`;
    }

    return progress;
  };

  const startStageTransition = () => {
    const elapsedTime = performance.now() - startTime;
    const remainingTime = Math.max(0, minDisplayTime - elapsedTime);

    setTimeout(() => {
      //Stage1:ローダーをフェードアウト
      if (loader) {
        loader.style.transition = `opacity ${loaderFadeDuration}ms ease-out`;
        loader.style.opacity = "0";
      }

      setTimeout(() => {
        //ローダーを削除
        loader?.remove();

        //Stage2:2段階目をフェードイン
        if (secondProcess) {
          secondProcess.style.transition = `opacity ${secondProcessFadeDuration}ms ease-in`;
          secondProcess.style.opacity = "1";
        }

        setTimeout(() => {
          //Stage3:2段階目をフェードアウト(やっぱりメインコンテンツ先に表示しておく)
          if (secondProcess) {
            secondProcess.style.transition = `opacity ${secondProcessFadeDuration}ms ease-out`;
            secondProcess.style.opacity = "0";
            loadingScreen.style.transition = `opacity ${finalFadeDuration}ms ease-out`;
            loadingScreen.style.opacity = "0";

            if (mainContent) {
              mainContent.style.transition = `opacity ${finalFadeDuration}ms ease-in`;
              mainContent.style.opacity = "1";
            }
          }

          // Stage4:全体をフェードアウトとメインコンテンツをフェードイン
          setTimeout(() => {
            /*
            loadingScreen.style.transition = `opacity ${finalFadeDuration}ms ease-out`;
            loadingScreen.style.opacity = "0";

            if (mainContent) {
              mainContent.style.transition = `opacity ${finalFadeDuration}ms ease-in`;
              mainContent.style.opacity = "1";
            }
              */

            //最後はローディングの要素全部消す
            setTimeout(() => {
              loadingScreen.remove();
              onComplete?.();
            }, finalFadeDuration);
          }, secondProcessFadeDuration);
        }, secondProcessDisplayTime);
      }, loaderFadeDuration);
    }, remainingTime);
  };

  //画像の読み込みを追跡
  if (trackImages) {
    const images = Array.from(document.images);
    totalResources = images.length;

    if (totalResources === 0) {
      startStageTransition();
      return;
    }

    images.forEach((img) => {
      if (img.complete) {
        loadedResources++;
      } else {
        img.addEventListener("load", () => {
          loadedResources++;
          const progress = updateProgress();
          if (progress === 100) {
            startStageTransition();
          }
        });
        img.addEventListener("error", () => {
          loadedResources++;
          const progress = updateProgress();
          if (progress === 100) {
            startStageTransition();
          }
        });
      }
    });

    //初期プログレスを更新
    const initialProgress = updateProgress();
    if (initialProgress === 100) {
      startStageTransition();
    }
  } else {
    //通常のload待機
    if (document.readyState === "complete") {
      startStageTransition();
    } else {
      window.addEventListener("load", startStageTransition);
    }
  }
};
