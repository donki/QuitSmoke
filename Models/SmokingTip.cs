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
    public static List<SmokingTip> GetAllTips() => new()
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
        new SmokingTip { Title = "Ahorra dinero", Message = "Una cajetilla menos son aproximadamente $80 pesos que puedes usar en algo mejor.", Category = "Dinero", Icon = "💰" },
        new SmokingTip { Title = "Vacaciones gratis", Message = "El dinero que gastas en cigarros en un año podría pagarte unas vacaciones.", Category = "Dinero", Icon = "✈️" },
        new SmokingTip { Title = "Inversión en salud", Message = "El dinero ahorrado puedes invertirlo en un gimnasio o comida saludable.", Category = "Dinero", Icon = "🏋️" },
        new SmokingTip { Title = "Menos gastos médicos", Message = "Ahorrarás en consultas médicas y medicamentos relacionados con el tabaco.", Category = "Dinero", Icon = "🏥" },
        new SmokingTip { Title = "Mejor seguro", Message = "Los no fumadores pagan menos en seguros de vida y salud.", Category = "Dinero", Icon = "📋" },

        // Familia y social
        new SmokingTip { Title = "Protege a tu familia", Message = "El humo de segunda mano daña a tus seres queridos, especialmente niños.", Category = "Familia", Icon = "👨‍👩‍👧‍👦" },
        new SmokingTip { Title = "Mejor ejemplo", Message = "Eres un mejor ejemplo para tus hijos y personas que te admiran.", Category = "Familia", Icon = "⭐" },
        new SmokingTip { Title = "Más tiempo con familia", Message = "Vivirás más años para disfrutar con tus seres queridos.", Category = "Familia", Icon = "❤️" },
        new SmokingTip { Title = "Sin olor a cigarro", Message = "Tu ropa, casa y carro no olerán a cigarro.", Category = "Social", Icon = "🌸" },
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
        new SmokingTip { Title = "Identifica tus triggers", Message = "¿Qué situaciones te dan ganas de fumar? Prepárate para ellas.", Category = "Consejo", Icon = "🎯" },
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
        new SmokingTip { Title = "Menos enfermedades", Message = "Reduces drasticamente el riesgo de cáncer, infartos y derrames.", Category = "Largo plazo", Icon = "🏥" },
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
        new SmokingTip { Title = "Mejor citas", Message = "Muchas personas prefieren salir con no fumadores.", Category = "Social", Icon = "💕" },
        new SmokingTip { Title = "Líder positivo", Message = "Puedes ser un líder positivo en tu círculo social.", Category = "Social", Icon = "👑" },

        // Últimos consejos motivacionales
        new SmokingTip { Title = "Hoy es el día", Message = "Hoy puede ser el día que marque la diferencia en tu vida.", Category = "Motivación", Icon = "🌟" },
        new SmokingTip { Title = "Eres único", Message = "Tienes algo único que ofrecer al mundo. Cuídalo.", Category = "Motivación", Icon = "⭐" },
        new SmokingTip { Title = "El momento es ahora", Message = "No hay mejor momento que ahora para tomar una decisión saludable.", Category = "Motivación", Icon = "⏰" },
        new SmokingTip { Title = "Confía en ti", Message = "Has superado desafíos antes. Puedes superar este también.", Category = "Motivación", Icon = "🙏" },
        new SmokingTip { Title = "Tu historia", Message = "Tú escribes tu historia. Haz que sea una historia de superación.", Category = "Motivación", Icon = "📖" }
    };
}