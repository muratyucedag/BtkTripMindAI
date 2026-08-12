import {
    createUserMessage,
    createAgentMessage
} from "./chat.js";

import {
    updateCityCover,
    updateDestination
} from "./destination.js";

document.addEventListener("DOMContentLoaded", function () {

    const chatMessages = document.getElementById("chatMessages");
    const messageInput = document.getElementById("messageInput");
    const sendMessageButton = document.getElementById("sendMessageButton");
    const aiThinking = document.getElementById("aiThinking");
    const thinkingText = document.getElementById("thinkingText");
    const quickPromptButtons = document.querySelectorAll(".quick-prompt");

    //const detectedCity = document.getElementById("detectedCity");
    //const detectedCountry = document.getElementById("detectedCountry");
    //const destinationSeparator = document.getElementById("destinationSeparator");

    const currentTemperature = document.getElementById("currentTemperature");
    const currentCondition = document.getElementById("currentCondition");
    const feelsLikeTemperature = document.getElementById("feelsLikeTemperature");
    const currentWeatherIcon = document.getElementById("currentWeatherIcon");
    const forecastContainer = document.getElementById("forecastContainer");

   // const cityCoverImage = document.getElementById("cityCoverImage");

    const routeCard = document.getElementById("routeCard");
    const routeTitle = document.getElementById("routeTitle");
    const routeDaysContainer = document.getElementById("routeDaysContainer");

    const restaurantsContainer = document.getElementById("restaurantsContainer");
    const restaurantCount = document.getElementById("restaurantCount");

    const hotelsContainer = document.getElementById("hotelsContainer");
    const hotelCount = document.getElementById("hotelCount");

    const budgetItemsContainer = document.getElementById("budgetItemsContainer");
    const budgetTotal = document.getElementById("budgetTotal");
    const budgetSummary = document.getElementById("budgetSummary");
    const budgetLevelBadge = document.getElementById("budgetLevelBadge");

    function scrollChatToBottom() {
        chatMessages.scrollTop = chatMessages.scrollHeight;
    }

  

        function updateWeather(weather) {
        if (!weather) {
            return;
        }

        currentTemperature.textContent =
        `${Math.round(weather.temperature)}°C`;

        currentCondition.textContent =
        weather.condition || "Hava durumu bilgisi bulunamadı";

        feelsLikeTemperature.textContent =
        `Hissedilen: ${Math.round(weather.feelsLike)}°C`;

        if (weather.iconUrl) {
            currentWeatherIcon.src = weather.iconUrl;
        currentWeatherIcon.classList.remove("hidden");
        } else {
            currentWeatherIcon.removeAttribute("src");
        currentWeatherIcon.classList.add("hidden");
        }

        if (!Array.isArray(weather.forecasts)) {
            return;
        }

        forecastContainer.innerHTML = "";

        weather.forecasts.slice(0, 3).forEach(function (forecast) {
            const forecastItem =
        document.createElement("div");

        forecastItem.className = "text-center";

        const dayName =
        forecast.dayName || "-";

        const temperature =
        Math.round(forecast.averageTemperature);

        const minTemperature =
        Math.round(forecast.minTemperature);

        const maxTemperature =
        Math.round(forecast.maxTemperature);

        const iconHtml = forecast.iconUrl
        ? `
        <img src="${forecast.iconUrl}"
            alt="${forecast.condition || " Hava durumu"}"
                         class="w-8 h-8 mx-auto object-contain" />
        `
        : `
        <span class="material-symbols-outlined text-sm">
            cloud
        </span>
        `;

        forecastItem.innerHTML = `
        <p class="text-[10px] text-on-secondary-container/60 font-bold">
            ${dayName}
        </p>

        <div class="h-8 flex items-center justify-center">
            ${iconHtml}
        </div>

        <p class="text-[12px] font-bold">
            ${temperature}°
        </p>

        <p class="text-[9px] text-on-secondary-container/60">
            ${minTemperature}° / ${maxTemperature}°
        </p>
        `;

        forecastContainer.appendChild(forecastItem);
        });
    }

        function updateRestaurants(restaurantResult) {
        if (!restaurantResult ||
        !Array.isArray(restaurantResult.restaurants) ||
        restaurantResult.restaurants.length === 0) {

            return;
        }

        restaurantsContainer.innerHTML = "";

        restaurantCount.textContent =
        `${restaurantResult.restaurants.length} sonuç`;

        restaurantResult.restaurants.forEach(function (restaurant) {

            const restaurantCard =
        document.createElement("a");

        restaurantCard.className =
        "flex gap-3 p-3 bg-surface rounded-2xl " +
        "border border-outline-variant/20 " +
        "hover:border-primary/40 hover:shadow-sm " +
        "transition-all";

        if (restaurant.googleMapsUrl) {
            restaurantCard.href =
            restaurant.googleMapsUrl;

        restaurantCard.target = "_blank";
        restaurantCard.rel = "noopener noreferrer";
            }

        const iconContainer =
        document.createElement("div");

        iconContainer.className =
        "w-12 h-12 rounded-xl bg-primary-container/50 " +
        "flex items-center justify-center shrink-0";

        const icon =
        document.createElement("span");

        icon.className =
        "material-symbols-outlined text-primary";

        icon.textContent =
        "restaurant";

        iconContainer.appendChild(icon);

        const contentContainer =
        document.createElement("div");

        contentContainer.className =
        "flex-1 min-w-0";

        const titleRow =
        document.createElement("div");

        titleRow.className =
        "flex items-start justify-between gap-2";

        const restaurantName =
        document.createElement("p");

        restaurantName.className =
        "font-bold text-body-md text-on-surface truncate";

        restaurantName.textContent =
        restaurant.name || "Restoran";

        const ratingContainer =
        document.createElement("div");

        ratingContainer.className =
        "flex items-center gap-1 shrink-0 text-primary text-[11px] font-bold";

        const starIcon =
        document.createElement("span");

        starIcon.className =
        "material-symbols-outlined text-sm";

        starIcon.style.fontVariationSettings =
        "'FILL' 1";

        starIcon.textContent =
        "star";

        const ratingText =
        document.createElement("span");

        ratingText.textContent =
                restaurant.rating > 0
        ? restaurant.rating.toFixed(1)
        : "-";

        ratingContainer.appendChild(starIcon);
        ratingContainer.appendChild(ratingText);

        titleRow.appendChild(restaurantName);
        titleRow.appendChild(ratingContainer);

        const address =
        document.createElement("p");

        address.className =
        "text-[11px] text-on-surface-variant " +
        "line-clamp-2 mt-1";

        address.textContent =
        restaurant.address ||
        "Adres bilgisi bulunamadı";

        const details =
        document.createElement("div");

        details.className =
        "flex flex-wrap items-center gap-2 mt-2 " +
        "text-[10px] text-on-surface-variant";

        const priceBadge =
        document.createElement("span");

        priceBadge.className =
        "px-2 py-1 bg-white rounded-full " +
        "border border-outline-variant/20";

        priceBadge.textContent =
        restaurant.priceLevel ||
        "Fiyat bilgisi yok";

        const reviewBadge =
        document.createElement("span");

        reviewBadge.className =
        "px-2 py-1 bg-white rounded-full " +
        "border border-outline-variant/20";

        reviewBadge.textContent =
        `${restaurant.userRatingCount || 0} değerlendirme`;

        details.appendChild(priceBadge);
        details.appendChild(reviewBadge);

        contentContainer.appendChild(titleRow);
        contentContainer.appendChild(address);
        contentContainer.appendChild(details);

        restaurantCard.appendChild(iconContainer);
        restaurantCard.appendChild(contentContainer);

        restaurantsContainer.appendChild(restaurantCard);
        });
    }


        function updateHotels(hotelResult) {

        if (!hotelResult ||
        !Array.isArray(hotelResult.hotels) ||
        hotelResult.hotels.length === 0) {

            return;
        }

        hotelsContainer.innerHTML = "";

        hotelCount.textContent =
        `${hotelResult.hotels.length} sonuç`;

        hotelResult.hotels.forEach(function (hotel) {

            const hotelCard =
        document.createElement("a");

        hotelCard.className =
        "block bg-white border border-outline-variant/30 rounded-2xl overflow-hidden shadow-sm hover:shadow-md transition-shadow";

        if (hotel.googleMapsUrl) {
            hotelCard.href = hotel.googleMapsUrl;
        hotelCard.target = "_blank";
        hotelCard.rel = "noopener noreferrer";
            }

        const image =
        document.createElement("div");

        image.className =
        "h-32 bg-cover bg-center";

        image.style.backgroundImage =
        hotel.photoUrl
        ? `url('${hotel.photoUrl}')`
        : "linear-gradient(to right,#ececec,#d9d9d9)";

        const content =
        document.createElement("div");

        content.className = "p-4";

        content.innerHTML = `
        <div class="flex justify-between items-start mb-1">

            <h6 class="font-bold text-on-surface">
                ${hotel.name}
            </h6>

            <div class="flex items-center text-primary text-[12px] font-bold">

                <span class="material-symbols-outlined text-sm mr-0.5"
                    style="font-variation-settings:'FILL' 1;">
                    star
                </span>

                ${hotel.rating > 0 ? hotel.rating.toFixed(1) : "-"}

            </div>

        </div>

        <p class="text-[12px] text-on-surface-variant mb-2">
            ${hotel.address}
        </p>

        <p class="text-[11px] text-on-surface-variant">
            ${hotel.userRatingCount} değerlendirme
        </p>
        `;

        hotelCard.appendChild(image);
        hotelCard.appendChild(content);

        hotelsContainer.appendChild(hotelCard);
        });
    }

        function updateBudget(
        budgetResult,
        dayCount,
        adultCount,
        childCount,
        budgetLevel) {

        if (!budgetResult ||
        !Array.isArray(budgetResult.items) ||
        budgetResult.items.length === 0) {

            return;
        }

        budgetItemsContainer.innerHTML = "";

        const currency =
        budgetResult.currency || "EUR";

        const categorySettings = {
            Accommodation: {
            title: "Konaklama",
        icon: "hotel"
            },
        Food: {
            title: "Yemek",
        icon: "restaurant"
            },
        Transportation: {
            title: "Ulaşım",
        icon: "directions_bus"
            },
        Activities: {
            title: "Aktiviteler",
        icon: "local_activity"
            }
        };

        budgetResult.items.forEach(function (item) {

            const settings =
        categorySettings[item.category] || {
            title: item.category || "Diğer",
        icon: "payments"
                };

        const percentage =
        Math.max(
        0,
        Math.min(100, item.percentage || 0)
        );

        const budgetItem =
        document.createElement("div");

        budgetItem.innerHTML = `
        <div class="flex justify-between items-center text-[12px] mb-1">

            <div class="flex items-center gap-2">
                <span class="material-symbols-outlined text-sm text-primary">
                    ${settings.icon}
                </span>

                <span>
                    ${settings.title}
                </span>
            </div>

            <div class="flex items-center gap-2">

                <span class="font-bold text-on-surface">
                    ${formatBudgetAmount(item.amount, currency)}
                </span>

                <span class="text-on-surface-variant">
                    %${percentage}
                </span>
            </div>
        </div>

        <div class="h-2 w-full bg-surface rounded-full overflow-hidden">
            <div class="h-full bg-primary rounded-full transition-all duration-700"
                style="width: ${percentage}%">
            </div>
        </div>
        `;

        budgetItemsContainer.appendChild(budgetItem);
        });

        budgetTotal.textContent =
        formatBudgetAmount(
        budgetResult.totalAmount,
        currency
        );

        const personText =
        `${adultCount || 1} yetişkin` +
            ((childCount || 0) > 0
        ? `, ${childCount} çocuk`
        : "");

        budgetSummary.textContent =
        `${dayCount || 1} gün · ${personText}`;

        updateBudgetLevelBadge(budgetLevel);
    }

        function formatBudgetAmount(amount, currency) {

        const numericAmount =
        Number(amount) || 0;

        try {
            return new Intl.NumberFormat(
        "tr-TR",
        {
            style: "currency",
        currency: currency || "EUR",
        maximumFractionDigits: 0
                }
        ).format(numericAmount);
        } catch {
            return `${numericAmount.toLocaleString("tr-TR")} ${currency || "EUR"}`;
        }
    }

        function updateBudgetLevelBadge(budgetLevel) {

        if (!budgetLevel ||
        !budgetLevelBadge) {

            budgetLevelBadge.classList.add("hidden");
        return;
        }

        const levelSettings = {
            Economy: "Ekonomik",
        Medium: "Orta Segment",
        Luxury: "Lüks"
        };

        budgetLevelBadge.textContent =
        levelSettings[budgetLevel] ||
        budgetLevel;

        budgetLevelBadge.classList.remove("hidden");
    }


        function updateRoute(route) {
        if (!route ||
        !Array.isArray(route.days) ||
        route.days.length === 0) {
            return;
        }

        const dayCount =
        route.dayCount || route.days.length;

        routeTitle.textContent =
        `${dayCount} Günlük Rota`;

        routeDaysContainer.innerHTML = "";

        route.days.forEach(function (day, index) {
            const routeDayElement =
        document.createElement("div");

        routeDayElement.className =
        "flex gap-4 relative";

        const circleElement =
        document.createElement("div");

        circleElement.className =
        index === 0
        ? "w-6 h-6 bg-primary rounded-full border-4 border-white shadow-sm z-10 flex-shrink-0"
        : "w-6 h-6 bg-white rounded-full border-4 border-primary/30 shadow-sm z-10 flex-shrink-0";

        const contentElement =
        document.createElement("div");

        const titleElement =
        document.createElement("p");

        titleElement.className =
        index === 0
        ? "text-label-md font-bold text-primary uppercase"
        : "text-label-md font-bold text-on-surface-variant uppercase";

        titleElement.textContent =
        `${day.dayNumber}. Gün: ${day.title}`;

        const descriptionElement =
        document.createElement("p");

        descriptionElement.className =
        "text-body-md text-on-surface-variant";

        descriptionElement.textContent =
        day.description;

        contentElement.appendChild(titleElement);
        contentElement.appendChild(descriptionElement);

        routeDayElement.appendChild(circleElement);
        routeDayElement.appendChild(contentElement);

        routeDaysContainer.appendChild(routeDayElement);
        });
    }

        function getThinkingMessage(intent) {
            switch (intent) {
                case "Weather":
        return "Hava durumu bilgileri kontrol ediliyor...";

        case "Hotel":
        return "Size uygun oteller araştırılıyor...";

        case "Restaurant":
        return "Restoran önerileri hazırlanıyor...";

        case "Route":
        return "Seyahat rotanız oluşturuluyor...";

        case "Currency":
        return "Döviz bilgileri hesaplanıyor...";

        case "Flight":
        return "Uçuş seçenekleri araştırılıyor...";

        case "Transportation":
        return "Ulaşım seçenekleri inceleniyor...";

        case "Attraction":
        return "Gezilecek yerler hazırlanıyor...";

        case "CityRecommendation":
        return "Size uygun şehirler analiz ediliyor...";

        case "Budget":
        return "Seyahat bütçeniz hesaplanıyor...";

        default:
        return "Seyahat talebiniz analiz ediliyor...";
            }
        }

        function showThinking(message = "Seyahat talebiniz analiz ediliyor...") {
            thinkingText.textContent = message;
        aiThinking.classList.remove("hidden");
        scrollChatToBottom();
        }

        function hideThinking() {
            aiThinking.classList.add("hidden");
        }

        function setLoadingState(isLoading) {
            sendMessageButton.disabled = isLoading;
        messageInput.disabled = isLoading;

        if (isLoading) {
            sendMessageButton.classList.add(
                "opacity-60",
                "cursor-not-allowed"
            );

        messageInput.classList.add("opacity-70");
            } else {
            sendMessageButton.classList.remove(
                "opacity-60",
                "cursor-not-allowed"
            );

        messageInput.classList.remove("opacity-70");
            }
        }

        async function getAgentResponse(userMessage) {
            const response = await fetch("/Dashboard/SendMessage", {
            method: "POST",
        headers: {
            "Content-Type": "application/json"
                },
        body: JSON.stringify({
            message: userMessage
                })
            });

        let result;

        try {
            result = await response.json();
            } catch {
                throw new Error(
        "Sunucudan geçerli bir cevap alınamadı."
        );
            }

        if (!response.ok) {
                throw new Error(
        result.message ||
        "Sunucu isteği başarısız oldu."
        );
            }

        if (!result.isSuccess) {
                throw new Error(
        result.message ||
        "Mesaj işlenirken bir hata oluştu."
        );
            }

        return result;
        }

        async function sendMessage() {
            const message = messageInput.value.trim();

        if (message === "") {
            messageInput.focus();
        return;
            }

        createUserMessage(message);

        messageInput.value = "";

        setLoadingState(true);

        showThinking(
        "Seyahat talebiniz analiz ediliyor..."
        );

        scrollChatToBottom();

        try {
                const agentResult = await getAgentResponse(message);

        updateDestination(agentResult.city,agentResult.country);

        updateCityCover(agentResult.coverImageUrl);

        if (agentResult.intent === "Flight" &&
        agentResult.flight) {

            sessionStorage.setItem(
                "flightSearchResult",
                JSON.stringify(agentResult.flight)
            );

        sessionStorage.setItem(
        "flightSearchMeta",
        JSON.stringify({
            departureCity:
        agentResult.flight.departureAirportCode,
        arrivalCity:
        agentResult.flight.arrivalAirportCode,
        departureDate:
        agentResult.flight.departureDate,
        adultCount:
        agentResult.adultCount || 1,
        childCount:
        agentResult.childCount || 0,
        travelClass:
        "ECONOMY"
            })
        );

        window.location.href =
        "/Dashboard/FlightResults";

        return;
    }

        if (agentResult.intent === "Weather" && agentResult.weather)
        {
            updateWeather(agentResult.weather);
                }

        if (agentResult.intent === "Route" &&
        agentResult.route) {

            updateRoute(agentResult.route);
    }

        if (agentResult.intent === "Restaurant" &&
        agentResult.restaurant) {

            updateRestaurants(
                agentResult.restaurant
            );
    }

        if (agentResult.intent === "Hotel" &&
        agentResult.hotel) {

            updateHotels(
                agentResult.hotel
            );
    }

        if (agentResult.intent === "Budget" &&
        agentResult.budget) {

            updateBudget(
                agentResult.budget,
                agentResult.dayCount,
                agentResult.adultCount,
                agentResult.childCount,
                agentResult.budgetLevel
            );
    }

        /*
         * Backend cevabı geldikten sonra intent bilgisine
         * göre thinking metnini kısa süreli değiştiriyoruz.
         */
        thinkingText.textContent =
        getThinkingMessage(agentResult.intent);

        console.log("Agent sonucu:", agentResult);
        console.log("Intent:", agentResult.intent);
        console.log("Tool:", agentResult.toolName);
        console.log("Success:", agentResult.isSuccess);
        console.log("Message:", agentResult.message);

        /*
         * Aracın çalıştığını kullanıcıya kısa süre
         * göstermek için küçük bir bekleme ekledik.
         */
        await new Promise(function (resolve) {
            setTimeout(resolve, 700);
                });

        hideThinking();

        createAgentMessage(agentResult.message);
            } catch (error) {
            hideThinking();

        createAgentMessage(
        error.message ||
        "Üzgünüm, mesajınızı işlerken bir hata oluştu."
        );
            } finally {
            setLoadingState(false);

        messageInput.focus();
        scrollChatToBottom();
            }
        }

        sendMessageButton.addEventListener("click", function () {
            sendMessage();
        });

        messageInput.addEventListener("keydown", function (event) {
            if (event.key === "Enter" && !event.shiftKey) {
            event.preventDefault();
        sendMessage();
            }
        });

        quickPromptButtons.forEach(function (button) {
            button.addEventListener("click", function () {
                if (messageInput.disabled) {
                    return;
                }

                messageInput.value = button.textContent.trim();
                messageInput.focus();
            });
        });

        scrollChatToBottom();  
        messageInput.focus();
    });
