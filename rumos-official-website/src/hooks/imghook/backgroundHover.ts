export interface BackgroundHoverOptions {
  targetSelector: string;
  backgroundSelector: string;
  dataAttribute?: string;
  fadeDuration?: number;
}

export const initBackgroundHover = (options: BackgroundHoverOptions) => {
  const {
    targetSelector,
    backgroundSelector,
    dataAttribute = "data-bg-image",
    fadeDuration = 100,
  } = options;

  const bgElement = document.querySelector(backgroundSelector) as HTMLElement;
  if (!bgElement) {
    console.error(`Background element not found: ${backgroundSelector}`);
    return;
  }

  bgElement.style.transition = `opacity ${fadeDuration}ms ease-in-out`;
  bgElement.style.opacity = "0";

  const targets = document.querySelectorAll(targetSelector);
  let currentImageUrl: string | null = null;
  let isHovering = false;

  const imageCache = new Map<string, HTMLImageElement>();
  targets.forEach((target) => {
    const imageUrl = target.getAttribute(dataAttribute);
    if (imageUrl && !imageCache.has(imageUrl)) {
      const img = new Image();
      img.src = imageUrl;
      imageCache.set(imageUrl, img);
    }
  });

  targets.forEach((target) => {
    const imageUrl = target.getAttribute(dataAttribute);
    if (!imageUrl) return;

    target.addEventListener("mouseenter", () => {
      isHovering = true;

      if (currentImageUrl === imageUrl) {
        bgElement.style.opacity = "1";
        return;
      }

      //最初のhoverとフェードアウト完了後に切り替え
      if (!currentImageUrl) {
        //最初のhover即座に表示
        if (bgElement.tagName === "IMG") {
          (bgElement as HTMLImageElement).src = imageUrl;
        } else {
          bgElement.style.backgroundImage = `url(${imageUrl})`;
        }
        currentImageUrl = imageUrl;
        bgElement.style.opacity = "1";
      } else {
        //2つ目以降フェード切り替え
        bgElement.style.opacity = "0";
        setTimeout(() => {
          if (bgElement.tagName === "IMG") {
            (bgElement as HTMLImageElement).src = imageUrl;
          } else {
            bgElement.style.backgroundImage = `url(${imageUrl})`;
          }
          currentImageUrl = imageUrl;

          if (isHovering) {
            bgElement.style.opacity = "1";
          }
        }, fadeDuration);
      }
    });

    target.addEventListener("mouseleave", () => {
      isHovering = false;
      bgElement.style.opacity = "0";
      setTimeout(() => {
        if (!isHovering) {
          currentImageUrl = null;
        }
      }, fadeDuration);
    });
  });
};
