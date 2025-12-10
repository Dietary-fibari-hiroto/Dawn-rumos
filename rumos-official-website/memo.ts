// hooks/scrollFade.ts

export interface ScrollFadeOptions {
  selector?: string; // 対象要素のセレクタ
  threshold?: number; // 表示判定の閾値（0.0 〜 1.0）
  rootMargin?: string; // 判定エリアのマージン
  showClass?: string; // 表示時に追加するクラス
  hideClass?: string; // 非表示時に追加するクラス
  once?: boolean; // 一度だけ表示するか
  delay?: number; // 遅延時間（ms）
  stagger?: number; // 複数要素の遅延間隔（ms）
}

export const initScrollFade = (options: ScrollFadeOptions = {}) => {
  const {
    selector = ".scroll-fade",
    threshold = 0.2,
    rootMargin = "0px 0px -20% 0px",
    showClass = "show",
    hideClass = "",
    once = false,
    delay = 0,
    stagger = 0,
  } = options;

  const elements = document.querySelectorAll(selector);

  if (elements.length === 0) {
    console.warn(`No elements found with selector: ${selector}`);
    return;
  }

  const observer = new IntersectionObserver(
    (entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting) {
          // 画面内に入った
          const target = entry.target as HTMLElement;
          const elementDelay =
            delay + stagger * parseInt(target.dataset.index || "0");

          setTimeout(() => {
            target.classList.add(showClass);
            if (hideClass) {
              target.classList.remove(hideClass);
            }
          }, elementDelay);

          // 一度だけ表示する場合は監視を解除
          if (once) {
            observer.unobserve(entry.target);
          }
        } else {
          // 画面外に出た
          if (!once) {
            const target = entry.target as HTMLElement;
            target.classList.remove(showClass);
            if (hideClass) {
              target.classList.add(hideClass);
            }
          }
        }
      });
    },
    {
      threshold,
      rootMargin,
    }
  );

  // 各要素を監視
  elements.forEach((element, index) => {
    // stagger用のインデックスを設定
    (element as HTMLElement).dataset.index = index.toString();
    observer.observe(element);
  });

  // クリーンアップ関数を返す
  return () => {
    observer.disconnect();
  };
};

// シンプル版（80%の位置で判定）
export const initScrollFadeSimple = () => {
  const elements = document.querySelectorAll(".scroll-fade");

  elements.forEach((element) => {
    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            entry.target.classList.add("show");
          } else {
            entry.target.classList.remove("show");
          }
        });
      },
      {
        threshold: 0.2,
        rootMargin: "0px 0px -20% 0px", // 画面の80%の位置で判定
      }
    );

    observer.observe(element);
  });
};

// スクロールイベント版（Intersection Observer非対応ブラウザ用）
export const initScrollFadeLegacy = (options: ScrollFadeOptions = {}) => {
  const {
    selector = ".scroll-fade",
    showClass = "show",
    hideClass = "",
  } = options;

  const elements = document.querySelectorAll(selector);

  const handleScroll = () => {
    elements.forEach((element) => {
      const rect = element.getBoundingClientRect();
      const isVisible = rect.top < window.innerHeight * 0.8;

      if (isVisible) {
        element.classList.add(showClass);
        if (hideClass) {
          element.classList.remove(hideClass);
        }
      } else {
        element.classList.remove(showClass);
        if (hideClass) {
          element.classList.add(hideClass);
        }
      }
    });
  };

  window.addEventListener("scroll", handleScroll);
  handleScroll(); // 初期チェック

  return () => {
    window.removeEventListener("scroll", handleScroll);
  };
};
