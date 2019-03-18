export class UriHelper {
    public static getUri(path: string, parameters: { [key: string]: string } = null): string {
        let uri = path.trim();

        if (parameters) {
            // Add other parameters
            for (const p in parameters) {
                if (parameters.hasOwnProperty(p) && parameters[p]) {
                    if (uri.indexOf('?') > -1) {
                        uri += '&';
                    } else {
                        uri += '?';
                    }
                    uri += p + '=' + parameters[p];
                }
            }
        }

        return uri;
    }
}
