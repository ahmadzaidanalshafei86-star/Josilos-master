const baseUrl = window.appBaseUrl || '/';

document.addEventListener('DOMContentLoaded', () => {
    const ordersContainer = document.getElementById('ordersContainer');
    const searchMobile = document.getElementById('searchMobile');
    const sortDate = document.getElementById('sortDate');
    const filterStatus = document.getElementById('filterStatus');
    const applyFilters = document.getElementById('applyFilters');

    // Define translations for en-US and ar-JO
    const translations = {
        'en-JO': {
            Order: 'Order',
            Customer: 'Customer',
            Phone: 'Phone',
            Address: 'Address',
            Location: 'Location',
            ViewOnMap: 'View on Map',
            Payment: 'Payment',
            Total: 'Total',
            CreatedAt: 'Created At',
            Status: 'Status',
            Items: 'Items',
            Included: 'Included',
            Excluded: 'Excluded',
            MarkAsCompleted: 'Mark as Completed',
            MarkAsCancel: 'Mark as Cancel',
            Completed: 'Completed',
            Cancelled: 'Cancelled',
            Pending: 'Pending',
            FailedToCompleteOrder: 'Failed to complete order',
            ErrorCompletingOrder: 'Error completing order',
            FailedToCancelOrder: 'Failed to cancel order',
            ErrorCancelingOrder: 'Error canceling order',
            FailedToFetchOrders: 'Failed to fetch orders',
            ErrorFetchingOrders: 'Error fetching orders'
        },
        'ar-JO': {
            Order: 'الطلب',
            Customer: 'العميل',
            Phone: 'الهاتف',
            Address: 'العنوان',
            Location: 'الموقع',
            ViewOnMap: 'عرض على الخريطة',
            Payment: 'الدفع',
            Total: 'الإجمالي',
            CreatedAt: 'تاريخ الإنشاء',
            Status: 'الحالة',
            Items: 'العناصر',
            Included: 'مشمول',
            Excluded: 'مستبعد',
            MarkAsCompleted: 'وضع علامة كمكتمل',
            MarkAsCancel: 'وضع علامة كملغى',
            Completed: 'مكتمل',
            Cancelled: 'ملغى',
            Pending: 'قيد الانتظار',
            FailedToCompleteOrder: 'فشل في إكمال الطلب',
            ErrorCompletingOrder: 'خطأ في إكمال الطلب',
            FailedToCancelOrder: 'فشل في إلغاء الطلب',
            ErrorCancelingOrder: 'خطأ في إلغاء الطلب',
            FailedToFetchOrders: 'فشل في جلب الطلبات',
            ErrorFetchingOrders: 'خطأ في جلب الطلبات'
        }
    };

    // Select translations based on current culture, default to en-JO
    const currentCulture = window.currentCulture || 'en-JO';
    const t = translations[currentCulture] || translations['en-JO'];

    // Ensure SignalR connection is established
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/orderHub")
        .build();

    // Track whether the audio context is unlocked
    let isAudioUnlocked = false;

    // Preload a silent audio to unlock the audio context
    const welcomeAudio = new Audio('/sounds/welcome.mp3');

    function unlockAudioContext() {
        if (!isAudioUnlocked) {
            welcomeAudio.play().then(() => {
                isAudioUnlocked = true;
                console.log('Audio context unlocked successfully');
            }).catch(err => {
                console.error('Failed to unlock audio context:', err);
                console.log('Silent audio source:', welcomeAudio.src);
                console.log('Silent audio error:', welcomeAudio.error);
            });
        }
    }

    // Set up listeners for user interactions to unlock audio
    document.addEventListener('click', unlockAudioContext, { once: true });
    document.addEventListener('keydown', unlockAudioContext, { once: true });

    // Define sortOrders and filterOrders globally with DOM checks
    function sortOrders() {
        const sortDate = document.getElementById('sortDate');
        const container = document.getElementById('ordersContainer');
        if (!sortDate || !container) {
            console.warn('sortOrders: sortDate or ordersContainer not found in DOM');
            return;
        }

        const sortValue = sortDate.value;
        const cards = Array.from(container.querySelectorAll('.order-card'));

        cards.sort((a, b) => {
            const dateA = new Date(a.dataset.date);
            const dateB = new Date(b.dataset.date);
            return sortValue === 'newest' ? dateB - dateA : dateA - dateB;
        });

        container.innerHTML = '';
        cards.forEach(card => container.appendChild(card));
    }

    function filterOrders() {
        const searchMobile = document.getElementById('searchMobile');
        const container = document.getElementById('ordersContainer');
        if (!searchMobile || !container) {
            console.warn('filterOrders: searchMobile or ordersContainer not found in DOM');
            return;
        }

        const mobile = searchMobile.value.toLowerCase();
        const cards = document.querySelectorAll('.order-card');

        cards.forEach(card => {
            const cardMobile = card.dataset.mobile.toLowerCase();
            const mobileMatch = !mobile || cardMobile.includes(mobile);
            card.style.display = mobileMatch ? '' : 'none';
        });
    }

    // Handle Complete and Cancel Order
    ordersContainer.addEventListener('click', async (e) => {
        if (e.target.classList.contains('complete-order')) {
            const orderId = e.target.dataset.orderId;
            const CompleteOrderUrl = `${baseUrl}EsAdmin/Orders/CompleteOrder/${orderId}`;
            try {
                const response = await fetch(CompleteOrderUrl, { method: 'POST' });
                if (response.ok) {
                    e.target.closest('.order-card').remove();
                } else {
                    alert(t.FailedToCompleteOrder);
                }
            } catch (err) {
                console.error('Error completing order:', err);
                alert(t.ErrorCompletingOrder);
            }
        }

        if (e.target.classList.contains('cancel-order')) {
            const orderId = e.target.dataset.orderId;
            try {
                const CancelOrderUrl = `${baseUrl}EsAdmin/Orders/CancelOrder/${orderId}`;
                const response = await fetch(CancelOrderUrl, { method: 'POST' });
                if (response.ok) {
                    e.target.closest('.order-card').remove();
                } else {
                    alert(t.FailedToCancelOrder);
                }
            } catch (err) {
                console.error('Error canceling order:', err);
                alert(t.ErrorCancelingOrder);
            }
        }
    });

    // Handle Filters and Sorting via API
    applyFilters.addEventListener('click', async () => {
        const mobile = searchMobile.value.trim();
        const sort = sortDate.value;
        const status = filterStatus.value;

        try {
            const GetFilteredOrdersUrl = `${baseUrl}EsAdmin/Orders/GetFilteredOrders?mobile=${encodeURIComponent(mobile)}&sort=${sort}&status=${status}`;
            const response = await fetch(GetFilteredOrdersUrl);
            if (response.ok) {
                const orders = await response.json();
                renderOrders(orders);
            } else {
                alert(t.FailedToFetchOrders);
            }
        } catch (err) {
            console.error('Error fetching orders:', err);
            alert(t.ErrorFetchingOrders);
        }
    });
    function formatCurrency(amount) {
        const culture = currentCulture;
        let symbol = '';
        let formattedAmount = parseFloat(amount);

        if (isNaN(formattedAmount)) {
            return '';  // If not a valid number, return an empty string
        }

        switch (culture) {
            case "en-US":
                symbol = "JOD";
                return `${symbol} ${formattedAmount.toFixed(2)}`;
            case "ar-JO":
                symbol = "د.أ";
                return `${formattedAmount.toFixed(2)} ${symbol}`;
            default:
                symbol = "JOD"; // fallback
                return `${symbol} ${formattedAmount.toFixed(2)}`;
        }
    }




    // SignalR new order handler
    connection.on("ReceiveOrder", async (orderId) => {
        try {
            const GetOrderUrl = `${baseUrl}EsAdmin/Orders/GetOrder/${orderId}`;
            const response = await fetch(GetOrderUrl);
            const order = await response.json();

            if (order.isDelivered) return;

            const ordersContainer = document.getElementById('ordersContainer');
            if (!ordersContainer) {
                console.error('ReceiveOrder: ordersContainer not found in DOM');
                return;
            }

            // Create card with 'new-order' class for visual notification
            const statusBadgeClass = order.isDelivered ? 'bg-success' : order.isCancelled ? 'bg-danger' : 'bg-warning';
            const statusText = order.isDelivered ? t.Completed : order.isCancelled ? t.Cancelled : t.Pending;
            const buttons = (!order.isDelivered && !order.isCancelled) ? `
                <button class="btn btn-success w-100 mb-2 complete-order" data-order-id="${order.id}">${t.MarkAsCompleted}</button>
                <button class="btn btn-danger w-100 cancel-order" data-order-id="${order.id}">${t.MarkAsCancel}</button>
            ` : '';

            const itemsHtml = order.items.map(item => `
                    <li class="list-group-item">
                        <strong>${item.productTitle}${item.customizationName ? ` (${item.customizationName})` : ''}</strong> X (${item.quantity})
                        ${item.selectedAttributes.length ? `<div class="text-muted small">${t.Included}: ${item.selectedAttributes.map(a => a.value).join(", ")}</div>` : ""}
                        ${item.excludedAttributes.length ? `<div class="text-muted small">${t.Excluded}: ${item.excludedAttributes.map(a => a.value).join(", ")}</div>` : ""}
                    </li>
                `).join('');

            // Format date based on culture
            const dateOptions = currentCulture === 'ar-JO' ?
                { year: 'numeric', month: 'numeric', day: 'numeric', hour: 'numeric', minute: 'numeric', hour12: true } :
                { year: 'numeric', month: 'numeric', day: 'numeric', hour: 'numeric', minute: 'numeric', hour12: true };
            const formattedDate = new Date(order.createdAt).toLocaleString(currentCulture, dateOptions);

            const cardHtml = `
                    <div class="col-md-4 mb-4 order-card new-order" data-mobile="${order.mobilePhone}" data-date="${new Date(order.createdAt).toISOString()}">
                        <div class="card shadow-sm">
                            <div class="card-header bg-primary text-white">
                                <h5 class="card-title mb-0">${t.Order} #${order.id}</h5>
                            </div>
                            <div class="card-body">
                                <dl class="row mb-0">
                                    <dt class="col-sm-4">${t.Customer}</dt>
                                    <dd class="col-sm-8">${order.name}</dd>
                                    <dt class="col-sm-4">${t.Phone}</dt>
                                    <dd class="col-sm-8">${order.mobilePhone}</dd>
                                    <dt class="col-sm-4">${t.Address}</dt>
                                    <dd class="col-sm-8">${order.address}</dd>
                                    <dt class="col-sm-4">${t.Location}</dt>
                                    <dd class="col-sm-8"><a href="${order.locationLink}" target="_blank">${t.ViewOnMap}</a></dd>
                                    <dt class="col-sm-4">${t.Payment}</dt>
                                    <dd class="col-sm-8">${order.paymentMethod}</dd>
                                    <dt class="col-sm-4">
                                        ${formatCurrency(t.Total)} ${currentCulture === 'ar-JO' ? 'الإجمالي' : 'Total'}
                                    </dt>
                                    <dd class="col-sm-8">
                                        ${formatCurrency(order.estimatedTotal)} 
                                    </dd>
                                    <dt class="col-sm-4">${t.CreatedAt}</dt>
                                    <dd class="col-sm-8">${formattedDate}</dd>
                                    <dt class="col-sm-4">${t.Status}</dt>
                                    <dd class="col-sm-8">
                                        <span class="badge ${statusBadgeClass}">${statusText}</span>
                                    </dd>
                                </dl>
                                <h6 class="mt-3">${t.Items}</h6>
                                <ul class="list-group list-group-flush mb-3">${itemsHtml}</ul>
                                ${buttons}
                            </div>
                        </div>
                    </div>
                `;
            ordersContainer.insertAdjacentHTML("afterbegin", cardHtml);

            // Play notification sound
            const audio = new Audio('/CMS/sounds/notification.mp3');
            audio.play().catch(err => {
                console.warn('Notification audio playback failed:', err);
                // Enhance visual notification as fallback
                const newCard = ordersContainer.querySelector('.new-order');
                if (newCard) {
                    newCard.classList.add('new-order-fallback');
                    setTimeout(() => newCard.classList.remove('new-order', 'new-order-fallback'), 5000);
                }
            });

            // Remove 'new-order' class after animation
            const newCard = ordersContainer.querySelector('.new-order');
            if (newCard) {
                setTimeout(() => newCard.classList.remove('new-order'), 5000);
            }

            sortOrders();
        } catch (err) {
            window.location.reload();
            audio.play();
            console.error('Error handling new order:', err);
        }
    });

    // Function to Render Orders for Filtering
    function renderOrders(orders) {
        ordersContainer.innerHTML = '';

        orders.forEach(order => {
            const statusBadgeClass = order.isDelivered ? 'bg-success' : order.isCancelled ? 'bg-danger' : 'bg-warning';
            const statusText = order.isDelivered ? t.Completed : order.isCancelled ? t.Cancelled : t.Pending;

            const buttons = (!order.isDelivered && !order.isCancelled) ? `
            <button class="btn btn-success w-100 mb-2 d-flex align-items-center justify-content-center complete-order" data-order-id="${order.id}">
                <i class="lni lni-checkmark-circle me-2"></i> ${t.MarkAsCompleted}
            </button>
            <button class="btn btn-danger w-100 d-flex align-items-center justify-content-center cancel-order" data-order-id="${order.id}">
                <i class="lni lni-close me-2"></i> ${t.MarkAsCancel}
            </button>
        ` : '';

            const itemsHtml = order.items.map(item => `
            <li class="list-group-item">
                <strong>${item.productTitle}${item.customizationName ? ` (${item.customizationName})` : ''}</strong> X (${item.quantity})
                ${item.selectedAttributes.length ? `<div class="text-muted small">${t.Included}: ${item.selectedAttributes.map(a => a.value).join(", ")}</div>` : ""}
            </li>
        `).join('');

            // Adjust time by +3 hours
            const date = new Date(order.createdAt);
            date.setHours(date.getHours() + 3);

            const dateOptions = currentCulture === 'ar-JO'
                ? { year: 'numeric', month: 'numeric', day: 'numeric', hour: 'numeric', minute: 'numeric', hour12: true }
                : { year: 'numeric', month: 'numeric', day: 'numeric', hour: 'numeric', minute: 'numeric', hour12: true };

            const formattedDate = date.toLocaleString(currentCulture, dateOptions);

            const orderHtml = `
            <div class="col-md-4 mb-4 order-card" data-mobile="${order.mobilePhone}" data-date="${new Date(order.createdAt).toISOString()}">
                <div class="card shadow-sm">
                    <div class="card-header bg-primary text-white">
                        <h5 class="card-title mb-0">${t.Order} #${order.id}</h5>
                    </div>
                    <div class="card-body">
                        <dl class="row mb-0">
                            <dt class="col-sm-4">${t.Customer}</dt>
                            <dd class="col-sm-8">${order.customerFullName}</dd>
                            <dt class="col-sm-4">${t.Phone}</dt>
                            <dd class="col-sm-8">${order.mobilePhone}</dd>
                            <dt class="col-sm-4">${t.Address}</dt>
                            <dd class="col-sm-8">${order.streetAddress}</dd>
                            <dt class="col-sm-4">${t.Payment}</dt>
                            <dd class="col-sm-8">${order.paymentMethod}</dd>
                            <dt class="col-sm-4">${t.Total}</dt>
                            <dd class="col-sm-8">${order.estimatedTotal.toFixed(2)}</dd>
                            <dt class="col-sm-4">${t.CreatedAt}</dt>
                            <dd class="col-sm-8">${formattedDate}</dd>
                            <dt class="col-sm-4">${t.Status}</dt>
                            <dd class="col-sm-8">
                                <span class="badge ${statusBadgeClass}">${statusText}</span>
                            </dd>
                        </dl>
                        <h6 class="mt-3">${t.Items}</h6>
                        <ul class="list-group list-group-flush mb-3">${itemsHtml}</ul>
                        ${buttons}
                    </div>
                </div>
            </div>
        `;
            ordersContainer.insertAdjacentHTML('beforeend', orderHtml);
        });
    }


    // Start SignalR connection
    connection.start().catch(err => console.error('SignalR connection failed:', err));

    // Initial sort
    sortOrders();
});