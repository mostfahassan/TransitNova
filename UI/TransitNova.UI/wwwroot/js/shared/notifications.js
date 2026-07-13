(() => {
    const body = document.body;
    if (!body) return;

    const pageUrl = body.dataset.notificationsPageUrl;
    const sessionUrl = body.dataset.notificationsSessionUrl;
    const isNotificationsPage = body.dataset.notificationsPage === 'true';
    const trigger = document.querySelector('[data-notifications-trigger]');
    const badge = document.querySelector('[data-notification-badge]');
    const feed = document.querySelector('[data-notifications-feed]');
    const list = document.querySelector('[data-notifications-list]');
    const emptyState = document.querySelector('[data-notifications-empty]');
    const pageSize = Number.parseInt(feed?.dataset.notificationsPageSize || '20', 10) || 20;

    if (!sessionUrl || !trigger || !badge) return;

    let cachedSession = null;
    let cachedAt = 0;

    const unreadCardClass = 'rounded-[18px] border border-[#F5D48B] bg-[#FFFBEA] p-5 shadow-sm';
    const readCardClass = 'rounded-[18px] border border-[#E5E7EB] bg-white p-5 shadow-sm';
    const unreadStateClass = 'inline-flex min-w-[72px] justify-center rounded-full border border-[#F59E0B] bg-[#FFF7ED] px-2.5 py-1 text-[11px] font-semibold text-[#B45309]';
    const readStateClass = 'inline-flex min-w-[72px] justify-center rounded-full border border-[#D1D5DB] bg-[#F8FAFC] px-2.5 py-1 text-[11px] font-semibold text-[#475569]';

    function setBadge(count) {
        const safeCount = Number.isFinite(count) ? Math.max(0, count) : 0;
        badge.textContent = String(safeCount);
        badge.classList.toggle('hidden', safeCount === 0);
    }

    function escapeHtml(value) {
        return String(value ?? '')
            .replaceAll('&', '&amp;')
            .replaceAll('<', '&lt;')
            .replaceAll('>', '&gt;')
            .replaceAll('"', '&quot;')
            .replaceAll("'", '&#39;');
    }

    function formatTimestamp(value) {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) return value ?? '';

        return new Intl.DateTimeFormat(undefined, {
            dateStyle: 'medium',
            timeStyle: 'short'
        }).format(date);
    }

    async function getSession(forceRefresh = false) {
        const now = Date.now();
        if (!forceRefresh && cachedSession && now - cachedAt < 30000) {
            return cachedSession;
        }

        const response = await fetch(sessionUrl, {
            method: 'GET',
            credentials: 'same-origin',
            headers: { Accept: 'application/json' }
        });

        const contentType = response.headers.get('content-type') || '';
        if (!response.ok || !contentType.includes('application/json')) {
            cachedSession = null;
            cachedAt = 0;
            return null;
        }

        cachedSession = await response.json();
        cachedAt = now;
        return cachedSession;
    }

    function apiUrl(session, path) {
        return `${String(session.apiBaseUrl).replace(/\/$/, '')}${path.startsWith('/') ? path : `/${path}`}`;
    }

    async function fetchApi(path, options = {}, retry = true) {
        const session = await getSession(options.forceRefresh === true);
        if (!session?.accessToken || !session?.apiBaseUrl) return null;

        const response = await fetch(apiUrl(session, path), {
            method: options.method || 'GET',
            headers: {
                Accept: 'application/json',
                Authorization: `Bearer ${session.accessToken}`,
                ...(options.headers || {})
            },
            body: options.body,
            credentials: 'include'
        });

        if (response.status === 401 && retry) {
            cachedSession = null;
            cachedAt = 0;
            return fetchApi(path, { ...options, forceRefresh: true }, false);
        }

        if (!response.ok) return null;
        if (response.status === 204) return {};

        const contentType = response.headers.get('content-type') || '';
        return contentType.includes('application/json') ? response.json() : {};
    }

    async function syncUnreadCount() {
        const payload = await fetchApi('/api/v1/notifications/unread-count');
        const count = payload?.data?.count ?? payload?.Data?.Count ?? 0;
        setBadge(count);
        return count;
    }

    async function markAllRead() {
        const response = await fetchApi('/api/v1/notifications/read-all', { method: 'PATCH' });
        if (response === null) return false;

        setBadge(0);
        markRenderedItemsRead();
        return true;
    }

    function applyReadState(article, isRead) {
        if (!article) return;

        article.dataset.read = isRead ? 'true' : 'false';
        article.className = isRead ? readCardClass : unreadCardClass;

        const state = article.querySelector('[data-notification-state]');
        if (state) {
            state.className = isRead ? readStateClass : unreadStateClass;
            state.textContent = isRead ? 'Read' : 'Unread';
        }
    }

    function markRenderedItemsRead() {
        document.querySelectorAll('[data-notification-item]').forEach((article) => applyReadState(article, true));
    }

    function hasUnreadRenderedItems() {
        return Array.from(document.querySelectorAll('[data-notification-item]')).some((article) => article.dataset.read !== 'true');
    }

    function renderNotification(notification) {
        const isRead = notification?.isRead === true || notification?.IsRead === true;
        const title = notification?.title ?? notification?.Title ?? 'Notification';
        const message = notification?.message ?? notification?.Message ?? '';
        const createdOnUtc = notification?.createdOnUtc ?? notification?.CreatedOnUtc ?? new Date().toISOString();

        return `
            <article data-notification-item data-read="${isRead ? 'true' : 'false'}" class="${isRead ? readCardClass : unreadCardClass}">
                <div class="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
                    <div class="min-w-0 flex-1">
                        <h3 class="text-base font-semibold text-[#111111]">${escapeHtml(title)}</h3>
                        <p class="mt-2 text-sm leading-6 text-[#4B5563]">${escapeHtml(message)}</p>
                    </div>
                    <span data-notification-state class="${isRead ? readStateClass : unreadStateClass}">${isRead ? 'Read' : 'Unread'}</span>
                </div>
                <p class="mt-3 text-xs font-semibold uppercase tracking-[0.08em] text-[#6B7280]">${escapeHtml(formatTimestamp(createdOnUtc))}</p>
            </article>`;
    }

    function prependNotification(notification) {
        if (!list) return;

        list.classList.remove('hidden');
        emptyState?.classList.add('hidden');
        list.insertAdjacentHTML('afterbegin', renderNotification(notification));

        while (list.children.length > pageSize) {
            list.removeChild(list.lastElementChild);
        }
    }

    async function handleIncomingNotification(notification) {
        if (isNotificationsPage) {
            prependNotification({ ...notification, isRead: true, IsRead: true });
            await markAllRead();
            return;
        }

        const currentCount = Number.parseInt(badge.textContent || '0', 10) || 0;
        setBadge(currentCount + 1);
    }

    async function buildConnection() {
        if (!window.signalR) return null;

        const session = await getSession();
        if (!session?.apiBaseUrl) return null;

        return new window.signalR.HubConnectionBuilder()
            .withUrl(apiUrl(session, '/hubs/notifications'), {
                accessTokenFactory: async () => {
                    const latestSession = await getSession(true);
                    return latestSession?.accessToken || '';
                }
            })
            .withAutomaticReconnect()
            .build();
    }

    async function reconnectSync() {
        await syncUnreadCount();
        if (isNotificationsPage && pageUrl) {
            window.location.assign(pageUrl);
        }
    }

    async function init() {
        const unreadCount = await syncUnreadCount();

        if (isNotificationsPage && (unreadCount > 0 || hasUnreadRenderedItems())) {
            await markAllRead();
        }

        const connection = await buildConnection();
        if (!connection) return;

        connection.on('ReceiveNotification', async (notification) => {
            await handleIncomingNotification(notification);
        });

        connection.onreconnecting(() => {
            body.dataset.notificationsConnection = 'reconnecting';
        });

        connection.onreconnected(async () => {
            body.dataset.notificationsConnection = 'connected';
            await reconnectSync();
        });

        connection.onclose(() => {
            body.dataset.notificationsConnection = 'disconnected';
        });

        try {
            await connection.start();
            body.dataset.notificationsConnection = 'connected';
        } catch {
            body.dataset.notificationsConnection = 'failed';
            // Best effort; the badge syncs again on the next page load or reconnect.
        }
    }

    init();
})();
