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
function getParamsInLocation(location: string, params: Param[]): string {
    let joinedParams = params.join("&");
    if (joinedParams.startsWith("&")) {
        joinedParams = joinedParams.substring(1);
    }
    let locat = location.split('?')[0] + "?" + joinedParams;
    return locat;
}
function getAllParams(paramsString: string): Param[] {
    let params: Param[] = new Array();

    let keyValueString = paramsString.split('&');
    //let statement did not work
    for (var i = 0; i < keyValueString.length; i++) {
        let split = keyValueString[i].split('=');

        params.push(new Param(split[0], split[1]));

    }

    return params;
}