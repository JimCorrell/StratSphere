// StratSphere site-wide JS — theme switching, tenant dropdown, inner tabs

document.addEventListener('DOMContentLoaded', function () {

    // ── Theme switcher ──────────────────────────────────────────────────────────
    // The inline <script> in _Layout.cshtml applies the saved theme before first
    // paint to avoid FOUC. These buttons update both the attribute and the cookie.
    document.querySelectorAll('.theme-btn').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var theme = btn.dataset.themeVal;
            document.documentElement.dataset.theme = theme;
            document.cookie = 'theme=' + theme + ';path=/;max-age=31536000;SameSite=Lax';
        });
    });

    // ── Tenant switcher dropdown ────────────────────────────────────────────────
    var switcher = document.getElementById('tenant-switcher');
    var menu     = document.getElementById('tenant-menu');
    if (switcher && menu) {
        switcher.addEventListener('click', function (e) {
            e.stopPropagation();
            menu.style.display = menu.style.display === 'none' ? 'block' : 'none';
        });
        // Prevent clicks inside the menu from closing it
        menu.addEventListener('click', function (e) {
            e.stopPropagation();
        });
        // Close on outside click
        document.addEventListener('click', function () {
            menu.style.display = 'none';
        });
    }

    // ── Inner-tab switcher ──────────────────────────────────────────────────────
    // Tabs use data-tab attribute; clicking appends ?tab=<value> to the URL so
    // the server can render the correct tab panel without a JS framework.
    document.querySelectorAll('.inner-tabs button[data-tab]').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var url = new URL(window.location.href);
            url.searchParams.set('tab', btn.dataset.tab);
            window.location.href = url.toString();
        });
    });

    // ── Standings view switcher ─────────────────────────────────────────────────
    // Same pattern but uses ?view= param for the standings page sub-views.
    document.querySelectorAll('.view-tabs button[data-view]').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var url = new URL(window.location.href);
            url.searchParams.set('view', btn.dataset.view);
            window.location.href = url.toString();
        });
    });

});
