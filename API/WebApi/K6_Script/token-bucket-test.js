import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    vus: 1,              // 模擬 1 個使用者
    iterations: 60,      // 共發送 60 次請求
};

export default function () {
    let url = 'https://localhost:44319/api/Health/HealthCheck';
    let res = http.get(url);

    // 印出回應的狀態碼與部分 body（避免太長）
    console.log(`Request to ${url} → ${res.status} | Body: ${res.body.slice(0, 100)}`);

    check(res, {
        'status is 200 or 429': (r) => r.status === 200 || r.status === 429,
        'rate limited (token bucket)': (r) =>
            r.status === 429 && r.body.includes('token bucket'),
    });

    sleep(0.05); // 稍微延遲
}
