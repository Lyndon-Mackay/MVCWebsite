class Param {
    name: string;
    value: string;
    constructor(name: string, value: string) {
        this.name = name;
        this.value = value;
    }
    toString(): string {
        if (this.name.length > 1 && this.value.length > 1) {
            return this.name + "=" + this.value;
        }
        return "";
    }
}
/**
 * readding all params to a variable containing a future url.
 * returns the url with params set by the array of Params objects
 * @param location the current href
 * @param params list of all prams in current href and added params
 */
function getParamsInLocation(location: string, params: Param[]): string { 
    return location.split('?')[0] + "?" + params.join("&");
}
/**
 * extracting all params from string into an array of params
 * @param paramsString the string should be the url after the "?"
 */
function getAllParams(paramsString: string): Param[] {
    let params: Param[] = new Array();

    if (paramsString == "") {
        return params;
    }
    let keyValueString = paramsString.split('&');
    //let statement did not work
    for (var i = 0; i < keyValueString.length; i++) {
        let split = keyValueString[i].split('=');

        params.push(new Param(split[0], split[1]));

    }
    return params;
}