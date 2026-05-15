document.addEventListener("DOMContentLoaded", () => {
  document.querySelectorAll("[data-confirm]").forEach((form) => {
    form.addEventListener("submit", (event) => {
      const message = form.getAttribute("data-confirm") || "Are you sure?";
      if (!window.confirm(message)) {
        event.preventDefault();
      }
    });
  });

  const tableFilter = document.querySelector("[data-table-filter]");
  if (tableFilter) {
    const targetSelector = tableFilter.getAttribute("data-table-filter");
    const rows = document.querySelectorAll(`${targetSelector} tbody tr`);
    tableFilter.addEventListener("input", () => {
      const query = tableFilter.value.toLowerCase();
      rows.forEach((row) => {
        row.hidden = !row.innerText.toLowerCase().includes(query);
      });
    });
  }

  const cartBadge = document.getElementById("cartCountBadge");
  if (cartBadge && window.bookStoreUserIsSignedIn) {
    fetch("/Cart/Count", { headers: { "Accept": "application/json" } })
      .then((response) => response.ok ? response.json() : { count: 0 })
      .then((data) => {
        cartBadge.textContent = data.count > 0 ? data.count : "";
      })
      .catch(() => {
        cartBadge.textContent = "";
      });
  }
});
