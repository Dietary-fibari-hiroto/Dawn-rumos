// hooks/loadingScreen.ts

export interface LoadingScreenOptions {
  loadingScreenId?: string;
  mainContentId?: string;
  minDisplayTime?: number; // 最低表示時間（ms）
  fadeOutDuration?: number; // フェードアウト時間（ms）
  onComplete?: () => void; // 完了時のコールバック
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

  if (!loadingScreen) {
    console.warn(`Loading screen element not found: #${loadingScreenId}`);
    return;
  }

  const startTime = performance.now();

  const hideLoading = () => {
    const elapsedTime = performance.now() - startTime;
    const remainingTime = Math.max(0, minDisplayTime - elapsedTime);

    setTimeout(() => {
      // フェードアウト開始
      loadingScreen.classList.add("hidden");
      mainContent?.classList.add("visible");

      // アニメーション完了後に削除
      setTimeout(() => {
        loadingScreen.remove();
        onComplete?.();
      }, fadeOutDuration);
    }, remainingTime);
  };

  // ページ読み込み完了を待つ
  if (document.readyState === "complete") {
    hideLoading();
  } else {
    window.addEventListener("load", hideLoading);
  }
};

export interface LoadingScreenWithProgressOptions extends LoadingScreenOptions {
  progressBarId?: string;
  progressTextId?: string;
  trackImages?: boolean; //画像の読み込みを追跡するか
  trackResources?: boolean; //その他のリソースも追跡するか
}

// プログレスバー付き + 段階的フェード版
export interface LoadingScreenStageOptions
  extends LoadingScreenWithProgressOptions {
  loaderSelector?: string; // ローダー要素のセレクタ
  secondProcessSelector?: string; // 2段階目の要素のセレクタ
  loaderFadeDuration?: number; // ローダーのフェード時間（ms）
  secondProcessFadeDuration?: number; // 2段階目のフェード時間（ms）
  secondProcessDisplayTime?: number; // 2段階目の表示時間（ms）
  finalFadeDuration?: number; // 最終フェードアウト時間（ms）
  showOnce?: boolean; // セッション中一度だけ表示するか
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
    showOnce = true, // デフォルトで一度だけ表示
    onComplete,
  } = options;

  const loadingScreen = document.getElementById(loadingScreenId);
  const mainContent = document.getElementById(mainContentId);

  if (!loadingScreen) return;

  // セッション中に既に表示済みかチェック
  if (showOnce && sessionStorage.getItem("loadingShown") === "true") {
    // 即座に削除してメインコンテンツを表示
    loadingScreen.remove();
    if (mainContent) {
      mainContent.style.opacity = "1";
    }
    onComplete?.();
    return;
  }

  const progressBar = document.getElementById(progressBarId) as HTMLElement;
  const progressText = document.getElementById(progressTextId) as HTMLElement;
  const loader = document.querySelector(loaderSelector) as HTMLElement;
  const secondProcess = document.querySelector(
    secondProcessSelector
  ) as HTMLElement;

  // 2段階目を最初は非表示に
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
      // Stage 1: ローダーをフェードアウト
      if (loader) {
        loader.style.transition = `opacity ${loaderFadeDuration}ms ease-out`;
        loader.style.opacity = "0";
      }

      setTimeout(() => {
        // ローダーを削除
        loader?.remove();

        // Stage 2: 2段階目をフェードイン
        if (secondProcess) {
          secondProcess.style.transition = `opacity ${secondProcessFadeDuration}ms ease-in`;
          secondProcess.style.opacity = "1";
        }

        setTimeout(() => {
          // Stage 3: 2段階目をフェードアウト
          if (secondProcess) {
            secondProcess.style.transition = `opacity ${secondProcessFadeDuration}ms ease-out`;
            secondProcess.style.opacity = "0";
          }

          // Stage 4: 全体をフェードアウト & メインコンテンツをフェードイン
          setTimeout(() => {
            loadingScreen.style.transition = `opacity ${finalFadeDuration}ms ease-out`;
            loadingScreen.style.opacity = "0";

            if (mainContent) {
              mainContent.style.transition = `opacity ${finalFadeDuration}ms ease-in`;
              mainContent.style.opacity = "1";
            }

            // 最終的に削除
            setTimeout(() => {
              loadingScreen.remove();

              // 表示済みフラグを立てる
              if (showOnce) {
                sessionStorage.setItem("loadingShown", "true");
              }

              onComplete?.();
            }, finalFadeDuration);
          }, secondProcessFadeDuration);
        }, secondProcessDisplayTime);
      }, loaderFadeDuration);
    }, remainingTime);
  };

  // 画像の読み込みを追跡
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

    // 初期プログレスを更新
    const initialProgress = updateProgress();
    if (initialProgress === 100) {
      startStageTransition();
    }
  } else {
    // 通常のload待機
    if (document.readyState === "complete") {
      startStageTransition();
    } else {
      window.addEventListener("load", startStageTransition);
    }
  }
};
