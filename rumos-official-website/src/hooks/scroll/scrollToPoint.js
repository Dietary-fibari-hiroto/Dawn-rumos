// src/utils/scroll.ts (または scroll.js)
export const scrollToPoint = (id) => {
  const el = document.getElementById(id);
  if (!el) return;
  const elRect = el.getBoundingClientRect();
  const absoluteElTop = elRect.top + window.scrollY;
  const centerPos = absoluteElTop - window.innerHeight / 2 + elRect.height / 2;
  window.scrollTo({
    top: centerPos,
    behavior: "smooth",
  });
};

export const initScrollCards = () => {
  document.querySelectorAll("[data-tech-id]").forEach((card) => {
    card.addEventListener("click", () => {
      const id = card.getAttribute("data-tech-id");
      if (id) scrollToPoint(id);
    });
  });
};
