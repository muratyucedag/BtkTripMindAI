const detectedCity = document.getElementById("detectedCity");

const detectedCountry = document.getElementById("detectedCountry");

const destinationSeparator = document.getElementById("destinationSeparator");

const cityCoverImage = document.getElementById("cityCoverImage");

export function updateCityCover(imageUrl) {
    if (!imageUrl || !cityCoverImage) {
        return;
    }

    const temporaryImage = new Image();

    temporaryImage.onload = function () {
        cityCoverImage.style.backgroundImage =
            `url("${imageUrl}")`;
    };

    temporaryImage.onerror = function () {
        console.warn(
            "Şehir kapak görseli yüklenemedi:",
            imageUrl
        );
    };

    temporaryImage.src = imageUrl;
}


//buradan kodu aldım



export function updateDestination(city, country) {
    const hasCity =
        typeof city === "string" &&
        city.trim() !== "";

    const hasCountry =
        typeof country === "string" &&
        country.trim() !== "";

    if (!hasCity && !hasCountry) {
        return;
    }

    if (hasCity) {
        detectedCity.textContent = city.trim();
    }

    if (hasCountry) {
        detectedCountry.textContent = country.trim();
    }

    if (destinationSeparator) {
        destinationSeparator.classList.toggle(
            "hidden",
            !hasCity || !hasCountry
        );
    }
}