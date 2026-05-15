document.addEventListener("DOMContentLoaded", () => {
  document.querySelectorAll("[data-confirm]").forEach((form) => {
    form.addEventListener("submit", (event) => {
      const message = form.getAttribute("data-confirm") || "Are you sure?";
      if (!window.confirm(message)) {
        event.preventDefault();
      }
    });
  });

  const setupTableFilterAndPager = (filterInput) => {
    const targetSelector = filterInput.getAttribute("data-table-filter");
    if (!targetSelector) return;

    const table = document.querySelector(targetSelector);
    const pager = document.querySelector(`[data-table-pager='${targetSelector}']`);
    if (!table || !pager) return;

    const rows = Array.from(table.querySelectorAll("tbody tr"));
    const pageSizeInput = pager.querySelector("[data-page-size]");
    const prevButton = pager.querySelector("[data-page-prev]");
    const nextButton = pager.querySelector("[data-page-next]");
    let pageIndex = 0;
    let maxPage = 0;

    const getPageSize = () => {
      const value = parseInt(pageSizeInput?.value ?? "10", 10);
      return Number.isNaN(value) ? 10 : Math.max(1, value);
    };

    const getFilteredRows = () => {
      const query = filterInput.value.trim().toLowerCase();
      if (!query) return rows;
      return rows.filter((row) => row.innerText.toLowerCase().includes(query));
    };

    const renderPage = () => {
      const filtered = getFilteredRows();
      const size = getPageSize();
      maxPage = Math.max(0, Math.ceil(filtered.length / size) - 1);
      pageIndex = Math.min(pageIndex, maxPage);

      const start = pageIndex * size;
      const end = start + size;

      rows.forEach((row) => {
        row.hidden = true;
      });

      filtered.forEach((row, index) => {
        row.hidden = index < start || index >= end;
      });

      if (prevButton) {
        prevButton.disabled = pageIndex === 0;
        prevButton.textContent = `Prev ${size}`;
      }

      if (nextButton) {
        nextButton.disabled = pageIndex >= maxPage;
        nextButton.textContent = `Next ${size}`;
      }
    };

    if (pageSizeInput) {
      pageSizeInput.addEventListener("input", () => {
        pageIndex = 0;
        renderPage();
      });
    }

    if (prevButton) {
      prevButton.addEventListener("click", () => {
        if (pageIndex > 0) {
          pageIndex -= 1;
          renderPage();
        }
      });
    }

    if (nextButton) {
      nextButton.addEventListener("click", () => {
        if (pageIndex < maxPage) {
          pageIndex += 1;
          renderPage();
        }
      });
    }

    filterInput.addEventListener("input", () => {
      pageIndex = 0;
      renderPage();
    });

    renderPage();
  };

  document.querySelectorAll("[data-table-filter]").forEach(setupTableFilterAndPager);

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

  document.querySelectorAll("[data-requires-login='true']").forEach((link) => {
    link.addEventListener("click", (event) => {
      if (!window.bookStoreUserIsSignedIn) {
        event.preventDefault();
        const alertHost = document.getElementById("clientAlertHost");
        if (alertHost) {
          alertHost.innerHTML = `<div class="alert alert-login-notice alert-dismissible fade show" role="alert">
              Please login first, still you can explore view all pages.
              <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>`;
        }
      }
    });
  });
});
