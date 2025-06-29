import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    vus: 31,             // 31 個同時虛擬使用者
    duration: '10s',     // 持續 10 秒
};

export default function () {
    let url = 'https://localhost:44319/api/Health/HealthCheckDelay';
    let res = http.get(url);

    // 印出狀態碼與部分回應內容（避免爆版）
    console.log(`VU#${__VU} → ${res.status} | Body: ${res.body.slice(0, 80)}`);

    check(res, {
        'status is 200 or 429': (r) => r.status === 200 || r.status === 429,
        'rate limited (concurrency)': (r) =>
            r.status === 429 && r.body.includes('concurrent'),
    });

    sleep(0.1);
}
