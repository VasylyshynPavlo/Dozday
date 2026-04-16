window.auth = (function () {
    const tokenStorageKey = "auth_token";

    function setToken(token) {
        if (!token) {
            localStorage.removeItem(tokenStorageKey);
            return;
        }

        localStorage.setItem(tokenStorageKey, token);
    }

    function getToken() {
        return localStorage.getItem(tokenStorageKey);
    }

    function clearToken() {
        localStorage.removeItem(tokenStorageKey);
    }

    function waitForGoogleApi() {
        return new Promise((resolve, reject) => {
            const maxAttempts = 50;
            let attempts = 0;

            const timer = setInterval(() => {
                attempts++;

                if (window.google?.accounts?.oauth2) {
                    clearInterval(timer);
                    resolve();
                    return;
                }

                if (attempts >= maxAttempts) {
                    clearInterval(timer);
                    reject(new Error("Google API script is not loaded."));
                }
            }, 100);
        });
    }

    async function requestGoogleAccessToken(clientId, dotNetRef) {
        if (!clientId) {
            throw new Error("Google Client ID is missing.");
        }

        if (!dotNetRef) {
            throw new Error("DotNet callback reference is missing.");
        }

        await waitForGoogleApi();

        return new Promise((resolve, reject) => {
            const tokenClient = google.accounts.oauth2.initTokenClient({
                client_id: clientId,
                scope: "openid email profile https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile",
                prompt: "select_account",
                callback: async (tokenResponse) => {
                    try {
                        if (!tokenResponse?.access_token) {
                            reject(new Error("Google access token was not received."));
                            return;
                        }

                        await dotNetRef.invokeMethodAsync("HandleGoogleCredential", "", tokenResponse.access_token);
                        resolve();
                    } catch (error) {
                        reject(error);
                    }
                },
                error_callback: (error) => {
                    reject(new Error(error?.message ?? "Google authorization failed."));
                }
            });

            tokenClient.requestAccessToken();
        });
    }

    return {
        setToken,
        getToken,
        clearToken,
        requestGoogleAccessToken
    };
})();
