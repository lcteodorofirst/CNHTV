(function () {
  "use strict";

  var DEFAULT_API_BASE_URL = "https://itm.curitiba.cnh.com:7001/cnhtv/api";
  var APP_VERSION = "1.0.0";
  var DEFAULT_HEARTBEAT_SECONDS = 30;
  var MIN_LOCAL_REFRESH_MINUTES = 1;
  var STORAGE_KEY = "cnhtv.settings";

  var connectionScreen = document.getElementById("connectionScreen");
  var connectionMessage = document.getElementById("connectionMessage");
  var connectionWarning = document.getElementById("connectionWarning");
  var deviceIdentifier = document.getElementById("deviceIdentifier");
  var frame = document.getElementById("presentationFrame");
  var settingsButton = document.getElementById("settingsButton");
  var settingsDialog = document.getElementById("settingsDialog");
  var settingsForm = document.getElementById("settingsForm");
  var integrationEnabledInput = document.getElementById("integrationEnabled");
  var apiBaseUrlInput = document.getElementById("apiBaseUrl");
  var localPresentationUrlInput = document.getElementById("localPresentationUrl");
  var localRefreshEnabledInput = document.getElementById("localRefreshEnabled");
  var localRefreshMinutesInput = document.getElementById("localRefreshMinutes");
  var localSettingsPanel = document.getElementById("localSettingsPanel");
  var settingsError = document.getElementById("settingsError");
  var cancelSettingsButton = document.getElementById("cancelSettingsButton");
  var resetSettingsButton = document.getElementById("resetSettingsButton");

  var heartbeatTimer = null;
  var refreshTimer = null;
  var retryTimer = null;
  var warningTimer = null;
  var retrySeconds = 5;
  var currentConfigurationVersion = null;
  var currentUrl = null;
  var deviceKey = getOrCreateDeviceKey();
  var settings = loadSettings();

  deviceIdentifier.textContent = "Equipamento: " + deviceKey;
  bindEvents();
  startConfiguredMode();

  function defaultSettings() {
    return {
      integrationEnabled: true,
      apiBaseUrl: DEFAULT_API_BASE_URL,
      localPresentationUrl: "",
      localRefreshEnabled: false,
      localRefreshMinutes: 5
    };
  }

  function loadSettings() {
    var defaults = defaultSettings();
    try {
      var stored = JSON.parse(localStorage.getItem(STORAGE_KEY) || "{}");
      return {
        integrationEnabled: stored.integrationEnabled !== false,
        apiBaseUrl: normalizeBaseUrl(stored.apiBaseUrl || defaults.apiBaseUrl),
        localPresentationUrl: stored.localPresentationUrl || defaults.localPresentationUrl,
        localRefreshEnabled: stored.localRefreshEnabled === true,
        localRefreshMinutes: validRefreshMinutes(stored.localRefreshMinutes) ? Number(stored.localRefreshMinutes) : defaults.localRefreshMinutes
      };
    } catch (error) {
      return defaults;
    }
  }

  function saveSettings(nextSettings) {
    settings = nextSettings;
    localStorage.setItem(STORAGE_KEY, JSON.stringify(settings));
  }

  function getOrCreateDeviceKey() {
    var stored = localStorage.getItem("cnhtv.deviceKey");
    if (stored) return stored;
    var randomPart = Math.random().toString(36).slice(2) + Date.now().toString(36);
    var key = "LG-" + randomPart.toUpperCase();
    localStorage.setItem("cnhtv.deviceKey", key);
    return key;
  }

  function deviceInfo() {
    return {
      DeviceKey: deviceKey,
      Name: null,
      Model: navigator.platform || "LG webOS TV",
      WebOsVersion: extractWebOsVersion(navigator.userAgent),
      AppVersion: APP_VERSION
    };
  }

  function extractWebOsVersion(userAgent) {
    var match = /Web0S[^;)]*|webOS[^;)]*/i.exec(userAgent || "");
    return match ? match[0] : "Não identificado";
  }

  function bindEvents() {
    settingsButton.addEventListener("click", openSettings);
    cancelSettingsButton.addEventListener("click", closeSettings);
    resetSettingsButton.addEventListener("click", resetSettings);
    settingsForm.addEventListener("submit", submitSettings);
    integrationEnabledInput.addEventListener("change", updateSettingsAvailability);
    localRefreshEnabledInput.addEventListener("change", updateSettingsAvailability);
    document.addEventListener("keydown", handleRemoteKey);
  }

  function handleRemoteKey(event) {
    var keyCode = event.keyCode || event.which;
    var isRedButton = keyCode === 403;
    var isEscapeOrBack = event.key === "Escape" || keyCode === 27 || keyCode === 461;
    if (isRedButton) {
      event.preventDefault();
      openSettings();
      return;
    }
    if (isEscapeOrBack && isSettingsOpen()) {
      event.preventDefault();
      closeSettings();
    }
  }

  function openSettings() {
    integrationEnabledInput.checked = settings.integrationEnabled;
    apiBaseUrlInput.value = settings.apiBaseUrl;
    localPresentationUrlInput.value = settings.localPresentationUrl;
    localRefreshEnabledInput.checked = settings.localRefreshEnabled;
    localRefreshMinutesInput.value = settings.localRefreshMinutes;
    settingsError.textContent = "";
    updateSettingsAvailability();
    settingsDialog.classList.add("is-open");
    settingsDialog.setAttribute("aria-hidden", "false");
    setTimeout(function () { integrationEnabledInput.focus(); }, 0);
  }

  function closeSettings() {
    settingsDialog.classList.remove("is-open");
    settingsDialog.setAttribute("aria-hidden", "true");
    settingsButton.focus();
  }

  function isSettingsOpen() {
    return settingsDialog.classList.contains("is-open");
  }

  function updateSettingsAvailability() {
    var integrationEnabled = integrationEnabledInput.checked;
    apiBaseUrlInput.disabled = !integrationEnabled;
    localSettingsPanel.classList.toggle("is-disabled", integrationEnabled);
    localPresentationUrlInput.disabled = integrationEnabled;
    localRefreshEnabledInput.disabled = integrationEnabled;
    localRefreshMinutesInput.disabled = integrationEnabled || !localRefreshEnabledInput.checked;
  }

  function submitSettings(event) {
    event.preventDefault();
    var nextSettings = {
      integrationEnabled: integrationEnabledInput.checked,
      apiBaseUrl: normalizeBaseUrl(apiBaseUrlInput.value),
      localPresentationUrl: localPresentationUrlInput.value.trim(),
      localRefreshEnabled: localRefreshEnabledInput.checked,
      localRefreshMinutes: validRefreshMinutes(localRefreshMinutesInput.value) ? Number(localRefreshMinutesInput.value) : 5
    };
    var validationMessage = validateSettings(nextSettings);
    if (validationMessage) {
      settingsError.textContent = validationMessage;
      return;
    }
    saveSettings(nextSettings);
    closeSettings();
    startConfiguredMode();
  }

  function resetSettings() {
    var defaults = defaultSettings();
    integrationEnabledInput.checked = defaults.integrationEnabled;
    apiBaseUrlInput.value = defaults.apiBaseUrl;
    localPresentationUrlInput.value = defaults.localPresentationUrl;
    localRefreshEnabledInput.checked = defaults.localRefreshEnabled;
    localRefreshMinutesInput.value = defaults.localRefreshMinutes;
    settingsError.textContent = "Clique em Salvar e aplicar para confirmar a restauração.";
    updateSettingsAvailability();
  }

  function validateSettings(value) {
    if (value.integrationEnabled && !isHttpUrl(value.apiBaseUrl)) {
      return "Informe uma URL válida para o serviço, iniciando com http:// ou https://.";
    }
    if (!value.integrationEnabled && !isHttpUrl(value.localPresentationUrl)) {
      return "Informe uma URL válida para a página local, iniciando com http:// ou https://.";
    }
    if (!value.integrationEnabled && value.localRefreshEnabled && !validRefreshMinutes(value.localRefreshMinutes)) {
      return "O intervalo de atualização deve ser de pelo menos 1 minuto.";
    }
    return "";
  }

  function isHttpUrl(value) {
    return /^https?:\/\/[^\s]+$/i.test(value || "");
  }

  function validRefreshMinutes(value) {
    var number = Number(value);
    return isFinite(number) && number >= MIN_LOCAL_REFRESH_MINUTES;
  }

  function normalizeBaseUrl(value) {
    return String(value || "").trim().replace(/\/+$/, "");
  }

  function startConfiguredMode() {
    stopAllTimers();
    currentConfigurationVersion = null;
    currentUrl = null;
    connectionWarning.style.display = "none";
    if (settings.integrationEnabled) {
      showConnectionScreen("Conectando e registrando o equipamento...");
      connect();
    } else {
      startLocalPresentation();
    }
  }

  function stopAllTimers() {
    clearInterval(heartbeatTimer);
    clearInterval(refreshTimer);
    clearTimeout(retryTimer);
    clearTimeout(warningTimer);
    heartbeatTimer = null;
    refreshTimer = null;
    retryTimer = null;
  }

  function showConnectionScreen(message) {
    connectionMessage.textContent = message;
    connectionScreen.style.display = "flex";
    frame.style.display = "none";
  }

  function startLocalPresentation() {
    retrySeconds = 5;
    currentUrl = settings.localPresentationUrl;
    frame.src = currentUrl;
    frame.style.display = "block";
    connectionScreen.style.display = "none";
    if (settings.localRefreshEnabled) {
      refreshTimer = setInterval(function () {
        frame.src = appendCacheBuster(currentUrl);
      }, settings.localRefreshMinutes * 60 * 1000);
    }
  }

  function connect() {
    if (!settings.integrationEnabled) return;
    clearTimeout(retryTimer);
    connectionMessage.textContent = "Conectando e registrando o equipamento...";
    request("/display/connect", "POST", deviceInfo())
      .then(function (configuration) {
        if (!settings.integrationEnabled) return;
        retrySeconds = 5;
        connectionWarning.style.display = "none";
        applyServerConfiguration(configuration, true);
        scheduleHeartbeat(configuration.HeartbeatIntervalSeconds || DEFAULT_HEARTBEAT_SECONDS);
      })
      .catch(function () {
        if (!settings.integrationEnabled) return;
        connectionMessage.textContent = "Não foi possível conectar. Nova tentativa em " + retrySeconds + " segundos...";
        retryTimer = setTimeout(connect, retrySeconds * 1000);
        retrySeconds = Math.min(retrySeconds * 2, 60);
      });
  }

  function scheduleHeartbeat(intervalSeconds) {
    clearInterval(heartbeatTimer);
    heartbeatTimer = setInterval(heartbeat, Math.max(10, intervalSeconds) * 1000);
  }

  function heartbeat() {
    if (!settings.integrationEnabled) return;
    var info = deviceInfo();
    request("/display/" + encodeURIComponent(deviceKey) + "/heartbeat", "POST", {
      Model: info.Model,
      WebOsVersion: info.WebOsVersion,
      AppVersion: info.AppVersion
    })
      .then(function (configuration) {
        if (!settings.integrationEnabled) return;
        connectionWarning.style.display = "none";
        retrySeconds = 5;
        applyServerConfiguration(configuration, false);
      })
      .catch(function (error) {
        if (!settings.integrationEnabled) return;
        showWarning("Servidor temporariamente indisponível");
        if (error && error.status === 404) connect();
      });
  }

  function applyServerConfiguration(configuration, firstConnection) {
    var changed = currentConfigurationVersion !== configuration.ConfigurationUpdatedAtUtc ||
      currentUrl !== configuration.PresentationUrl;
    currentConfigurationVersion = configuration.ConfigurationUpdatedAtUtc;
    if (firstConnection || changed) {
      currentUrl = configuration.PresentationUrl;
      frame.src = currentUrl;
      frame.style.display = "block";
      connectionScreen.style.display = "none";
    }
    clearInterval(refreshTimer);
    if (configuration.RefreshEnabled) {
      refreshTimer = setInterval(function () {
        frame.src = appendCacheBuster(currentUrl);
      }, Math.max(30, configuration.RefreshIntervalSeconds) * 1000);
    }
  }

  function showWarning(message) {
    clearTimeout(warningTimer);
    connectionWarning.textContent = message;
    connectionWarning.style.display = "block";
    warningTimer = setTimeout(function () {
      connectionWarning.style.display = "none";
    }, 5000);
  }

  function appendCacheBuster(url) {
    var separator = url.indexOf("?") >= 0 ? "&" : "?";
    return url + separator + "_cnhtv=" + Date.now();
  }

  function request(path, method, body) {
    return fetch(settings.apiBaseUrl + path, {
      method: method,
      headers: { "Content-Type": "application/json", "Accept": "application/json" },
      body: JSON.stringify(body)
    }).then(function (response) {
      if (!response.ok) {
        var error = new Error("HTTP " + response.status);
        error.status = response.status;
        throw error;
      }
      return response.json();
    });
  }
})();
