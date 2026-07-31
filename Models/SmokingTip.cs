namespace QuitSmoke.Models;

public class SmokingTip
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Icon { get; set; } = "💡";
}

public static class SmokingTips
{
    /// <summary>Consejos en el idioma configurado ("es" o "en"). Cualquier otro cae a español.</summary>
    public static List<SmokingTip> GetAllTips(string lang = "es")
        => lang == "en" ? TipsEn : TipsEs;

    private static readonly List<SmokingTip> TipsEs = new()
    {
        // Salud física
        new SmokingTip { Title = "Tu respiración mejora", Message = "En solo 20 minutos sin fumar, tu frecuencia cardíaca y presión arterial bajan.", Category = "Salud", Icon = "🫁" },
        new SmokingTip { Title = "Mejor circulación", Message = "Después de 2-12 semanas sin fumar, tu circulación mejora y es más fácil hacer ejercicio.", Category = "Salud", Icon = "❤️" },
        new SmokingTip { Title = "Pulmones más limpios", Message = "En 1-9 meses, la tos y falta de aire disminuyen. Tus pulmones se limpian solos.", Category = "Salud", Icon = "🫁" },
        new SmokingTip { Title = "Menos riesgo de cáncer", Message = "Después de 1 año, tu riesgo de enfermedad cardíaca se reduce a la mitad.", Category = "Salud", Icon = "🛡️" },
        new SmokingTip { Title = "Mejor olfato", Message = "Tu sentido del olfato y gusto mejoran notablemente en las primeras semanas.", Category = "Salud", Icon = "👃" },
        new SmokingTip { Title = "Dientes más blancos", Message = "Tus dientes se vuelven más blancos y tu aliento es más fresco.", Category = "Salud", Icon = "🦷" },
        new SmokingTip { Title = "Piel más joven", Message = "Tu piel se ve más joven y saludable sin las toxinas del cigarro.", Category = "Salud", Icon = "✨" },
        new SmokingTip { Title = "Más energía", Message = "Tendrás más energía para hacer las cosas que realmente disfrutas.", Category = "Salud", Icon = "⚡" },
        new SmokingTip { Title = "Mejor sueño", Message = "Dormirás mejor sin la nicotina alterando tus ciclos de sueño.", Category = "Salud", Icon = "😴" },
        new SmokingTip { Title = "Sistema inmune fuerte", Message = "Tu sistema inmunológico se fortalece, enfermándote menos.", Category = "Salud", Icon = "🛡️" },

        // Dinero
        new SmokingTip { Title = "Ahorra dinero", Message = "Cada cajetilla que no compras es dinero que puedes usar en algo mejor.", Category = "Dinero", Icon = "💰" },
        new SmokingTip { Title = "Vacaciones gratis", Message = "El dinero que gastas en cigarros en un año podría pagarte unas vacaciones.", Category = "Dinero", Icon = "✈️" },
        new SmokingTip { Title = "Inversión en salud", Message = "El dinero ahorrado puedes invertirlo en un gimnasio o comida saludable.", Category = "Dinero", Icon = "🏋️" },
        new SmokingTip { Title = "Menos gastos médicos", Message = "Ahorrarás en consultas médicas y medicamentos relacionados con el tabaco.", Category = "Dinero", Icon = "🏥" },
        new SmokingTip { Title = "Mejor seguro", Message = "Los no fumadores pagan menos en seguros de vida y salud.", Category = "Dinero", Icon = "📋" },

        // Familia y social
        new SmokingTip { Title = "Protege a tu familia", Message = "El humo de segunda mano daña a tus seres queridos, especialmente niños.", Category = "Familia", Icon = "👨‍👩‍👧‍👦" },
        new SmokingTip { Title = "Mejor ejemplo", Message = "Eres un mejor ejemplo para tus hijos y personas que te admiran.", Category = "Familia", Icon = "⭐" },
        new SmokingTip { Title = "Más tiempo con familia", Message = "Vivirás más años para disfrutar con tus seres queridos.", Category = "Familia", Icon = "❤️" },
        new SmokingTip { Title = "Sin olor a cigarro", Message = "Tu ropa, casa y coche no olerán a cigarro.", Category = "Social", Icon = "🌸" },
        new SmokingTip { Title = "Mejor aliento", Message = "Tendrás mejor aliento para besos y conversaciones cercanas.", Category = "Social", Icon = "💋" },

        // Psicológico y emocional
        new SmokingTip { Title = "Más autocontrol", Message = "Demuestras que tienes control sobre tus impulsos y decisiones.", Category = "Mental", Icon = "🧠" },
        new SmokingTip { Title = "Menos ansiedad", Message = "Aunque parezca lo contrario, fumar aumenta la ansiedad a largo plazo.", Category = "Mental", Icon = "😌" },
        new SmokingTip { Title = "Mejor concentración", Message = "Sin las subidas y bajadas de nicotina, tu concentración es más estable.", Category = "Mental", Icon = "🎯" },
        new SmokingTip { Title = "Más confianza", Message = "Lograr dejar de fumar aumenta tu confianza en ti mismo.", Category = "Mental", Icon = "💪" },
        new SmokingTip { Title = "Menos estrés real", Message = "El cigarro solo alivia el estrés que él mismo causa.", Category = "Mental", Icon = "🧘" },

        // Actividades alternativas
        new SmokingTip { Title = "Toma agua", Message = "Bebe un vaso de agua. Te hidrata y ocupa tus manos y boca.", Category = "Alternativa", Icon = "💧" },
        new SmokingTip { Title = "Respira profundo", Message = "Haz 5 respiraciones profundas. Es más relajante que fumar.", Category = "Alternativa", Icon = "🌬️" },
        new SmokingTip { Title = "Camina 5 minutos", Message = "Una caminata corta libera endorfinas naturales.", Category = "Alternativa", Icon = "🚶" },
        new SmokingTip { Title = "Llama a un amigo", Message = "Conecta con alguien que te importa en lugar de fumar.", Category = "Alternativa", Icon = "📞" },
        new SmokingTip { Title = "Mastica chicle", Message = "Mantén tu boca ocupada con algo que no dañe tu salud.", Category = "Alternativa", Icon = "🍬" },
        new SmokingTip { Title = "Escucha música", Message = "Pon tu canción favorita y deja que mejore tu estado de ánimo.", Category = "Alternativa", Icon = "🎵" },
        new SmokingTip { Title = "Haz estiramientos", Message = "Estira tu cuerpo, libera tensión de forma saludable.", Category = "Alternativa", Icon = "🤸" },
        new SmokingTip { Title = "Come una fruta", Message = "Una manzana o naranja te da energía y sabor sin toxinas.", Category = "Alternativa", Icon = "🍎" },
        new SmokingTip { Title = "Medita 2 minutos", Message = "Cierra los ojos y enfócate en tu respiración.", Category = "Alternativa", Icon = "🧘‍♂️" },
        new SmokingTip { Title = "Escribe tus sentimientos", Message = "Anota qué sientes en lugar de fumar. Te ayuda a procesarlo.", Category = "Alternativa", Icon = "📝" },

        // Motivacionales
        new SmokingTip { Title = "Cada 'no' cuenta", Message = "Cada vez que dices no al cigarro, te haces más fuerte.", Category = "Motivación", Icon = "💪" },
        new SmokingTip { Title = "Ya llegaste hasta aquí", Message = "Has logrado reducir tu consumo. ¡No te rindas ahora!", Category = "Motivación", Icon = "🏆" },
        new SmokingTip { Title = "Piensa en tu 'por qué'", Message = "Recuerda la razón principal por la que decidiste dejar de fumar.", Category = "Motivación", Icon = "🎯" },
        new SmokingTip { Title = "Eres más fuerte", Message = "Eres más fuerte que cualquier adicción. Tienes el poder de elegir.", Category = "Motivación", Icon = "⚡" },
        new SmokingTip { Title = "El deseo pasará", Message = "Los antojos son temporales, pero los beneficios de no fumar son permanentes.", Category = "Motivación", Icon = "⏰" },

        // Datos curiosos
        new SmokingTip { Title = "20 minutos bastan", Message = "En solo 20 minutos sin fumar, tu cuerpo ya empieza a recuperarse.", Category = "Dato", Icon = "⏱️" },
        new SmokingTip { Title = "4000 químicos", Message = "Cada cigarro tiene más de 4000 químicos, 70 de ellos causan cáncer.", Category = "Dato", Icon = "⚗️" },
        new SmokingTip { Title = "Adicción rápida", Message = "La nicotina llega al cerebro en solo 10 segundos, pero el daño dura años.", Category = "Dato", Icon = "🧠" },
        new SmokingTip { Title = "Falsa relajación", Message = "Fumar no te relaja, solo alivia la ansiedad que la nicotina causa.", Category = "Dato", Icon = "🎭" },
        new SmokingTip { Title = "Mejor rendimiento", Message = "Los no fumadores tienen mejor rendimiento físico y mental.", Category = "Dato", Icon = "🏃" },

        // Consejos prácticos
        new SmokingTip { Title = "Cambia tu rutina", Message = "Si siempre fumas en cierto lugar, evítalo por un rato.", Category = "Consejo", Icon = "🔄" },
        new SmokingTip { Title = "Identifica tus desencadenantes", Message = "¿Qué situaciones te dan ganas de fumar? Prepárate para ellas.", Category = "Consejo", Icon = "🎯" },
        new SmokingTip { Title = "Recompénsate", Message = "Date un pequeño premio cada vez que resistes fumar.", Category = "Consejo", Icon = "🎁" },
        new SmokingTip { Title = "Busca apoyo", Message = "Habla con amigos o familia sobre tu proceso. No lo hagas solo.", Category = "Consejo", Icon = "🤝" },
        new SmokingTip { Title = "Ten paciencia", Message = "Cambiar un hábito toma tiempo. Sé paciente contigo mismo.", Category = "Consejo", Icon = "⏳" },

        // Beneficios inmediatos
        new SmokingTip { Title = "Mejor sabor", Message = "La comida sabe mejor cuando no fumas. Redescubre los sabores.", Category = "Inmediato", Icon = "🍽️" },
        new SmokingTip { Title = "Manos libres", Message = "Tus manos están libres para hacer cosas más productivas.", Category = "Inmediato", Icon = "👐" },
        new SmokingTip { Title = "Sin interrupciones", Message = "No tienes que interrumpir actividades para ir a fumar.", Category = "Inmediato", Icon = "⏸️" },
        new SmokingTip { Title = "Mejor imagen", Message = "Proyectas una imagen más saludable y profesional.", Category = "Inmediato", Icon = "👔" },
        new SmokingTip { Title = "Sin culpa", Message = "No sientes culpa después de fumar porque simplemente no lo haces.", Category = "Inmediato", Icon = "😊" },

        // Beneficios a largo plazo
        new SmokingTip { Title = "Vida más larga", Message = "Los no fumadores viven en promedio 10 años más.", Category = "Largo plazo", Icon = "📈" },
        new SmokingTip { Title = "Mejor vejez", Message = "Tendrás una vejez más saludable y activa.", Category = "Largo plazo", Icon = "👴" },
        new SmokingTip { Title = "Menos enfermedades", Message = "Reduces drásticamente el riesgo de cáncer, infartos y derrames.", Category = "Largo plazo", Icon = "🏥" },
        new SmokingTip { Title = "Mejor calidad de vida", Message = "Disfrutarás más de la vida sin las limitaciones del cigarro.", Category = "Largo plazo", Icon = "🌟" },
        new SmokingTip { Title = "Orgullo personal", Message = "Te sentirás orgulloso de haber vencido una adicción difícil.", Category = "Largo plazo", Icon = "🏅" },

        // Reflexiones
        new SmokingTip { Title = "¿Realmente lo necesitas?", Message = "Pregúntate: ¿realmente necesito este cigarro o es solo un hábito?", Category = "Reflexión", Icon = "🤔" },
        new SmokingTip { Title = "¿Cómo te sentirás después?", Message = "Piensa en cómo te sentirás después de fumar vs. después de resistir.", Category = "Reflexión", Icon = "💭" },
        new SmokingTip { Title = "¿Qué dirías a un amigo?", Message = "Si un amigo estuviera en tu situación, ¿le dirías que fume?", Category = "Reflexión", Icon = "👥" },
        new SmokingTip { Title = "¿Vale la pena?", Message = "¿Vale la pena los 5 minutos de 'placer' por años de daño?", Category = "Reflexión", Icon = "⚖️" },
        new SmokingTip { Title = "Tu futuro yo", Message = "¿Qué te agradecería más tu futuro yo: fumar ahora o resistir?", Category = "Reflexión", Icon = "🔮" },

        // Emocionales
        new SmokingTip { Title = "Eres valioso", Message = "Tu vida y salud son valiosas. Mereces cuidarte.", Category = "Emocional", Icon = "💎" },
        new SmokingTip { Title = "Tienes el control", Message = "Tú decides qué entra en tu cuerpo. Tienes el poder.", Category = "Emocional", Icon = "👑" },
        new SmokingTip { Title = "Cada día es nuevo", Message = "Cada día es una nueva oportunidad para tomar mejores decisiones.", Category = "Emocional", Icon = "🌅" },
        new SmokingTip { Title = "Eres un ejemplo", Message = "Alguien te está viendo y aprendiendo de tus decisiones.", Category = "Emocional", Icon = "👀" },
        new SmokingTip { Title = "Mereces amor propio", Message = "Cuidarte es la forma más pura de amor propio.", Category = "Emocional", Icon = "💝" },

        // Técnicas de distracción
        new SmokingTip { Title = "Cuenta hasta 100", Message = "Cuenta lentamente hasta 100. El antojo probablemente habrá pasado.", Category = "Técnica", Icon = "🔢" },
        new SmokingTip { Title = "Visualiza tu meta", Message = "Cierra los ojos e imagínate como una persona completamente libre del cigarro.", Category = "Técnica", Icon = "👁️" },
        new SmokingTip { Title = "Técnica 5-4-3-2-1", Message = "Nombra 5 cosas que ves, 4 que tocas, 3 que oyes, 2 que hueles, 1 que saboreas.", Category = "Técnica", Icon = "🔍" },
        new SmokingTip { Title = "Aprieta los puños", Message = "Aprieta los puños por 10 segundos, luego relaja. Libera la tensión.", Category = "Técnica", Icon = "✊" },
        new SmokingTip { Title = "Sonríe forzadamente", Message = "Sonríe aunque no tengas ganas. Tu cerebro liberará endorfinas.", Category = "Técnica", Icon = "😊" },

        // Recordatorios de progreso
        new SmokingTip { Title = "Mira tu progreso", Message = "Has reducido tu consumo. Cada cigarro que no fumas es una victoria.", Category = "Progreso", Icon = "📊" },
        new SmokingTip { Title = "Celebra los pequeños logros", Message = "Cada hora sin fumar es un logro que merece reconocimiento.", Category = "Progreso", Icon = "🎉" },
        new SmokingTip { Title = "Eres más fuerte que ayer", Message = "Cada día que practicas el autocontrol, te vuelves más fuerte.", Category = "Progreso", Icon = "💪" },
        new SmokingTip { Title = "El camino es el destino", Message = "No se trata de perfección, sino de progreso constante.", Category = "Progreso", Icon = "🛤️" },
        new SmokingTip { Title = "Pequeños pasos", Message = "Los grandes cambios se logran con pequeños pasos consistentes.", Category = "Progreso", Icon = "👣" },

        // Beneficios sociales
        new SmokingTip { Title = "Mejor conversación", Message = "Puedes tener conversaciones largas sin necesidad de interrumpir para fumar.", Category = "Social", Icon = "💬" },
        new SmokingTip { Title = "Más actividades", Message = "Puedes disfrutar de más lugares y actividades donde no se permite fumar.", Category = "Social", Icon = "🎭" },
        new SmokingTip { Title = "Sin discriminación", Message = "No enfrentas la discriminación social que a veces sufren los fumadores.", Category = "Social", Icon = "🤝" },
        new SmokingTip { Title = "Mejores citas", Message = "Muchas personas prefieren salir con no fumadores.", Category = "Social", Icon = "💕" },
        new SmokingTip { Title = "Líder positivo", Message = "Puedes ser un líder positivo en tu círculo social.", Category = "Social", Icon = "👑" },

        // Últimos consejos motivacionales
        new SmokingTip { Title = "Hoy es el día", Message = "Hoy puede ser el día que marque la diferencia en tu vida.", Category = "Motivación", Icon = "🌟" },
        new SmokingTip { Title = "Eres único", Message = "Tienes algo único que ofrecer al mundo. Cuídalo.", Category = "Motivación", Icon = "⭐" },
        new SmokingTip { Title = "El momento es ahora", Message = "No hay mejor momento que ahora para tomar una decisión saludable.", Category = "Motivación", Icon = "⏰" },
        new SmokingTip { Title = "Confía en ti", Message = "Has superado desafíos antes. Puedes superar este también.", Category = "Motivación", Icon = "🙏" },
        new SmokingTip { Title = "Tu historia", Message = "Tú escribes tu historia. Haz que sea una historia de superación.", Category = "Motivación", Icon = "📖" }
    };

    private static readonly List<SmokingTip> TipsEn = new()
    {
        // Physical health
        new SmokingTip { Title = "Your breathing improves", Message = "Just 20 minutes without smoking and your heart rate and blood pressure drop.", Category = "Health", Icon = "🫁" },
        new SmokingTip { Title = "Better circulation", Message = "After 2-12 weeks smoke-free, your circulation improves and exercise gets easier.", Category = "Health", Icon = "❤️" },
        new SmokingTip { Title = "Cleaner lungs", Message = "In 1-9 months, coughing and shortness of breath fade. Your lungs clean themselves.", Category = "Health", Icon = "🫁" },
        new SmokingTip { Title = "Lower cancer risk", Message = "After 1 year, your risk of heart disease drops by half.", Category = "Health", Icon = "🛡️" },
        new SmokingTip { Title = "Better sense of smell", Message = "Your sense of smell and taste improve noticeably in the first few weeks.", Category = "Health", Icon = "👃" },
        new SmokingTip { Title = "Whiter teeth", Message = "Your teeth get whiter and your breath fresher.", Category = "Health", Icon = "🦷" },
        new SmokingTip { Title = "Younger skin", Message = "Your skin looks younger and healthier without cigarette toxins.", Category = "Health", Icon = "✨" },
        new SmokingTip { Title = "More energy", Message = "You'll have more energy for the things you truly enjoy.", Category = "Health", Icon = "⚡" },
        new SmokingTip { Title = "Better sleep", Message = "You'll sleep better without nicotine disrupting your sleep cycles.", Category = "Health", Icon = "😴" },
        new SmokingTip { Title = "Stronger immune system", Message = "Your immune system gets stronger, so you get sick less often.", Category = "Health", Icon = "🛡️" },

        // Money
        new SmokingTip { Title = "Save money", Message = "Every pack you don't buy is money you can spend on something better.", Category = "Money", Icon = "💰" },
        new SmokingTip { Title = "A free holiday", Message = "What you spend on cigarettes in a year could pay for a holiday.", Category = "Money", Icon = "✈️" },
        new SmokingTip { Title = "Invest in health", Message = "You can put the money you save into a gym or healthy food.", Category = "Money", Icon = "🏋️" },
        new SmokingTip { Title = "Fewer medical costs", Message = "You'll save on doctor visits and tobacco-related medication.", Category = "Money", Icon = "🏥" },
        new SmokingTip { Title = "Cheaper insurance", Message = "Non-smokers pay less for life and health insurance.", Category = "Money", Icon = "📋" },

        // Family and social
        new SmokingTip { Title = "Protect your family", Message = "Second-hand smoke harms your loved ones, especially children.", Category = "Family", Icon = "👨‍👩‍👧‍👦" },
        new SmokingTip { Title = "A better example", Message = "You're a better example for your kids and those who look up to you.", Category = "Family", Icon = "⭐" },
        new SmokingTip { Title = "More time with family", Message = "You'll live more years to enjoy with your loved ones.", Category = "Family", Icon = "❤️" },
        new SmokingTip { Title = "No cigarette smell", Message = "Your clothes, home and car won't smell of smoke.", Category = "Social", Icon = "🌸" },
        new SmokingTip { Title = "Fresher breath", Message = "You'll have fresher breath for kisses and close conversations.", Category = "Social", Icon = "💋" },

        // Psychological and emotional
        new SmokingTip { Title = "More self-control", Message = "You show you're in control of your impulses and decisions.", Category = "Mental", Icon = "🧠" },
        new SmokingTip { Title = "Less anxiety", Message = "It may seem otherwise, but smoking increases anxiety in the long run.", Category = "Mental", Icon = "😌" },
        new SmokingTip { Title = "Better focus", Message = "Without nicotine's ups and downs, your focus is steadier.", Category = "Mental", Icon = "🎯" },
        new SmokingTip { Title = "More confidence", Message = "Managing to quit boosts your confidence in yourself.", Category = "Mental", Icon = "💪" },
        new SmokingTip { Title = "Less real stress", Message = "A cigarette only relieves the stress it causes in the first place.", Category = "Mental", Icon = "🧘" },

        // Alternative activities
        new SmokingTip { Title = "Drink water", Message = "Have a glass of water. It hydrates you and keeps your hands and mouth busy.", Category = "Alternative", Icon = "💧" },
        new SmokingTip { Title = "Breathe deeply", Message = "Take 5 deep breaths. It's more relaxing than a cigarette.", Category = "Alternative", Icon = "🌬️" },
        new SmokingTip { Title = "Walk for 5 minutes", Message = "A short walk releases natural endorphins.", Category = "Alternative", Icon = "🚶" },
        new SmokingTip { Title = "Call a friend", Message = "Connect with someone you care about instead of smoking.", Category = "Alternative", Icon = "📞" },
        new SmokingTip { Title = "Chew gum", Message = "Keep your mouth busy with something that won't harm your health.", Category = "Alternative", Icon = "🍬" },
        new SmokingTip { Title = "Listen to music", Message = "Play your favourite song and let it lift your mood.", Category = "Alternative", Icon = "🎵" },
        new SmokingTip { Title = "Do some stretches", Message = "Stretch your body and release tension in a healthy way.", Category = "Alternative", Icon = "🤸" },
        new SmokingTip { Title = "Eat some fruit", Message = "An apple or orange gives you energy and flavour without toxins.", Category = "Alternative", Icon = "🍎" },
        new SmokingTip { Title = "Meditate for 2 minutes", Message = "Close your eyes and focus on your breathing.", Category = "Alternative", Icon = "🧘‍♂️" },
        new SmokingTip { Title = "Write down your feelings", Message = "Note how you feel instead of smoking. It helps you process it.", Category = "Alternative", Icon = "📝" },

        // Motivational
        new SmokingTip { Title = "Every 'no' counts", Message = "Every time you say no to a cigarette, you get stronger.", Category = "Motivation", Icon = "💪" },
        new SmokingTip { Title = "You've come this far", Message = "You've already cut down. Don't give up now!", Category = "Motivation", Icon = "🏆" },
        new SmokingTip { Title = "Remember your 'why'", Message = "Recall the main reason you decided to quit smoking.", Category = "Motivation", Icon = "🎯" },
        new SmokingTip { Title = "You are stronger", Message = "You're stronger than any addiction. You have the power to choose.", Category = "Motivation", Icon = "⚡" },
        new SmokingTip { Title = "The craving will pass", Message = "Cravings are temporary, but the benefits of not smoking are permanent.", Category = "Motivation", Icon = "⏰" },

        // Fun facts
        new SmokingTip { Title = "20 minutes is enough", Message = "In just 20 minutes without smoking, your body already starts to recover.", Category = "Fact", Icon = "⏱️" },
        new SmokingTip { Title = "4,000 chemicals", Message = "Each cigarette has over 4,000 chemicals, 70 of which cause cancer.", Category = "Fact", Icon = "⚗️" },
        new SmokingTip { Title = "Fast addiction", Message = "Nicotine reaches the brain in just 10 seconds, but the damage lasts years.", Category = "Fact", Icon = "🧠" },
        new SmokingTip { Title = "False relaxation", Message = "Smoking doesn't relax you, it only eases the anxiety nicotine causes.", Category = "Fact", Icon = "🎭" },
        new SmokingTip { Title = "Better performance", Message = "Non-smokers have better physical and mental performance.", Category = "Fact", Icon = "🏃" },

        // Practical tips
        new SmokingTip { Title = "Change your routine", Message = "If you always smoke in a certain place, avoid it for a while.", Category = "Tip", Icon = "🔄" },
        new SmokingTip { Title = "Spot your triggers", Message = "Which situations make you want to smoke? Get ready for them.", Category = "Tip", Icon = "🎯" },
        new SmokingTip { Title = "Reward yourself", Message = "Give yourself a small treat every time you resist a cigarette.", Category = "Tip", Icon = "🎁" },
        new SmokingTip { Title = "Seek support", Message = "Talk to friends or family about your journey. Don't do it alone.", Category = "Tip", Icon = "🤝" },
        new SmokingTip { Title = "Be patient", Message = "Changing a habit takes time. Be patient with yourself.", Category = "Tip", Icon = "⏳" },

        // Immediate benefits
        new SmokingTip { Title = "Better taste", Message = "Food tastes better when you don't smoke. Rediscover the flavours.", Category = "Immediate", Icon = "🍽️" },
        new SmokingTip { Title = "Free hands", Message = "Your hands are free for more productive things.", Category = "Immediate", Icon = "👐" },
        new SmokingTip { Title = "No interruptions", Message = "You don't have to interrupt what you're doing to go and smoke.", Category = "Immediate", Icon = "⏸️" },
        new SmokingTip { Title = "Better image", Message = "You project a healthier, more professional image.", Category = "Immediate", Icon = "👔" },
        new SmokingTip { Title = "No guilt", Message = "You don't feel guilty after smoking because you simply don't.", Category = "Immediate", Icon = "😊" },

        // Long-term benefits
        new SmokingTip { Title = "A longer life", Message = "Non-smokers live on average 10 years longer.", Category = "Long term", Icon = "📈" },
        new SmokingTip { Title = "A better old age", Message = "You'll enjoy a healthier, more active old age.", Category = "Long term", Icon = "👴" },
        new SmokingTip { Title = "Fewer diseases", Message = "You drastically cut the risk of cancer, heart attacks and strokes.", Category = "Long term", Icon = "🏥" },
        new SmokingTip { Title = "Better quality of life", Message = "You'll enjoy life more without the limits of smoking.", Category = "Long term", Icon = "🌟" },
        new SmokingTip { Title = "Personal pride", Message = "You'll feel proud of having beaten a tough addiction.", Category = "Long term", Icon = "🏅" },

        // Reflections
        new SmokingTip { Title = "Do you really need it?", Message = "Ask yourself: do I really need this cigarette, or is it just a habit?", Category = "Reflection", Icon = "🤔" },
        new SmokingTip { Title = "How will you feel after?", Message = "Think about how you'll feel after smoking vs. after resisting.", Category = "Reflection", Icon = "💭" },
        new SmokingTip { Title = "What would you tell a friend?", Message = "If a friend were in your shoes, would you tell them to smoke?", Category = "Reflection", Icon = "👥" },
        new SmokingTip { Title = "Is it worth it?", Message = "Are 5 minutes of 'pleasure' worth years of damage?", Category = "Reflection", Icon = "⚖️" },
        new SmokingTip { Title = "Your future self", Message = "What would your future self thank you for more: smoking now or resisting?", Category = "Reflection", Icon = "🔮" },

        // Emotional
        new SmokingTip { Title = "You are valuable", Message = "Your life and health are valuable. You deserve to take care of yourself.", Category = "Emotional", Icon = "💎" },
        new SmokingTip { Title = "You're in control", Message = "You decide what goes into your body. You hold the power.", Category = "Emotional", Icon = "👑" },
        new SmokingTip { Title = "Every day is new", Message = "Every day is a fresh chance to make better choices.", Category = "Emotional", Icon = "🌅" },
        new SmokingTip { Title = "You're an example", Message = "Someone is watching and learning from your decisions.", Category = "Emotional", Icon = "👀" },
        new SmokingTip { Title = "You deserve self-love", Message = "Caring for yourself is the purest form of self-love.", Category = "Emotional", Icon = "💝" },

        // Distraction techniques
        new SmokingTip { Title = "Count to 100", Message = "Count slowly to 100. The craving will probably have passed.", Category = "Technique", Icon = "🔢" },
        new SmokingTip { Title = "Visualise your goal", Message = "Close your eyes and picture yourself completely free of cigarettes.", Category = "Technique", Icon = "👁️" },
        new SmokingTip { Title = "5-4-3-2-1 technique", Message = "Name 5 things you see, 4 you touch, 3 you hear, 2 you smell, 1 you taste.", Category = "Technique", Icon = "🔍" },
        new SmokingTip { Title = "Clench your fists", Message = "Clench your fists for 10 seconds, then relax. Release the tension.", Category = "Technique", Icon = "✊" },
        new SmokingTip { Title = "Force a smile", Message = "Smile even if you don't feel like it. Your brain will release endorphins.", Category = "Technique", Icon = "😊" },

        // Progress reminders
        new SmokingTip { Title = "Look at your progress", Message = "You've cut down. Every cigarette you don't smoke is a win.", Category = "Progress", Icon = "📊" },
        new SmokingTip { Title = "Celebrate small wins", Message = "Every hour without smoking is an achievement worth recognising.", Category = "Progress", Icon = "🎉" },
        new SmokingTip { Title = "Stronger than yesterday", Message = "Every day you practise self-control, you get stronger.", Category = "Progress", Icon = "💪" },
        new SmokingTip { Title = "The journey is the goal", Message = "It's not about perfection, but about steady progress.", Category = "Progress", Icon = "🛤️" },
        new SmokingTip { Title = "Small steps", Message = "Big changes are made with small, consistent steps.", Category = "Progress", Icon = "👣" },

        // Social benefits
        new SmokingTip { Title = "Better conversation", Message = "You can hold long conversations without breaking off to smoke.", Category = "Social", Icon = "💬" },
        new SmokingTip { Title = "More activities", Message = "You can enjoy more places and activities where smoking isn't allowed.", Category = "Social", Icon = "🎭" },
        new SmokingTip { Title = "No stigma", Message = "You don't face the social stigma smokers sometimes deal with.", Category = "Social", Icon = "🤝" },
        new SmokingTip { Title = "Better dates", Message = "Many people prefer to date non-smokers.", Category = "Social", Icon = "💕" },
        new SmokingTip { Title = "A positive leader", Message = "You can be a positive leader in your social circle.", Category = "Social", Icon = "👑" },

        // Final motivational tips
        new SmokingTip { Title = "Today is the day", Message = "Today could be the day that changes your life.", Category = "Motivation", Icon = "🌟" },
        new SmokingTip { Title = "You are unique", Message = "You have something unique to offer the world. Take care of it.", Category = "Motivation", Icon = "⭐" },
        new SmokingTip { Title = "The moment is now", Message = "There's no better time than now to make a healthy decision.", Category = "Motivation", Icon = "⏰" },
        new SmokingTip { Title = "Trust yourself", Message = "You've overcome challenges before. You can overcome this one too.", Category = "Motivation", Icon = "🙏" },
        new SmokingTip { Title = "Your story", Message = "You write your story. Make it a story of overcoming.", Category = "Motivation", Icon = "📖" }
    };
}
